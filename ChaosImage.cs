using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace ChaosIslandHacking
{
    class ChaosImage
    {
        //This class is not to be instanced, but rather, to be used for loading palettes as int[] and sprite files as standard Bitmaps.

        public struct Sprite
        {
            public int fType;
            public int width;
            public int height;
            public int dataWidth; //dataWidth is width rounded up to the nearest multiple of 4, except for fType == 0.
            public int subimages;
            public int stage1Size; //Buffer size necessary to run through stage 1 (non-RLE) decompression
            public AnimData[] animDataList;
            public int[] rowBeginnings;
            public byte[][] imageBlocks;
        }

        public struct AnimData
        {
            public byte unknown1;
            public byte unknown2;
            public byte unknown3;
            public byte unknown4;
        }



        private static uint abgrToArgb(uint abgrVal)
        {
            uint red = abgrVal & 0xFF;
            uint green = abgrVal & 0xFF00;
            uint blue = (abgrVal & 0xFF0000) >> 16;
            uint alpha = 0xFF000000 - (abgrVal & 0xFF000000); //Alpha is also opposite what it should be...well, technically, it's not alpha in the palette file in the first place; it's "flags"

            return alpha | (red << 16) | green | blue;
        }

        //TODO: May as well add transparency, team color, and shadow to the palette, huh? Though then I have to use Int16 instead of byte to store decompressed (stage 2) image data.
        public static int loadPalette(string filename, ref int[] palette) //I wanted to make palette an 'out', buuut then I have to assign to it even in the case of an error. Forget that.
        {
            byte[] wholeFile = File.ReadAllBytes(filename);

            if (wholeFile[0] != 'R' || wholeFile[1] != 'I' || wholeFile[2] != 'F' || wholeFile[3] != 'F')
            {
                return 1; //Not a Chaos Island PAL file (which is aaactually in Microsoft RIFF format)
            }

            BinaryReader br = new BinaryReader(new MemoryStream(wholeFile));

            try
            {
                br.ReadInt32(); //Don't care about the first 4 bytes
                int fileLen = br.ReadInt32();
                br.ReadInt32(); //Skip "PAL "
                br.ReadInt32(); //Skip "data"
                int chunkLen = br.ReadInt32();
                br.ReadInt16(); //Skip version (00 03)
                int colCount = br.ReadInt16();

                //Load the whole palette (R, G, B, flags)
                int[] pal = new int[259]; //256 colors in the file and 3 special ones, in case I want to go that route (but for now, the paletted pixel data consists of bytes)
                for (int colLoaded = 0; colLoaded < colCount; colLoaded++)
                {
                    pal[colLoaded] = (int)abgrToArgb((uint)br.ReadInt32()); //Had to rearrange bytes
                }

                pal[256] = Color.Transparent.ToArgb(); //Transparent
                pal[257] = 0x7F000000; //Semitransparent
                pal[258] = 0x7F7F7F7F; //Team color (dunno why I made it semitransparent though!)

                palette = pal;
            }
            catch
            {
                return 2; //Not enough data in file.
            }

            return 0; //No error.
        }

        public static int loadSprite(string filename, ref Sprite stillCompressedSprite)
        {
            byte[] wholeFile = File.ReadAllBytes(filename);

            BinaryReader br = new BinaryReader(new MemoryStream(wholeFile));

            stillCompressedSprite.fType = br.ReadInt32();
            if (stillCompressedSprite.fType != 0 && stillCompressedSprite.fType != 2 && stillCompressedSprite.fType != 3 && stillCompressedSprite.fType != 5)
            {
                return 1; //Not a Chaos Island SPR file (although the game supports types 1 and 4 according to the assembly, there are none in the archives)
            }

            stillCompressedSprite.width = br.ReadInt32();
            stillCompressedSprite.height = br.ReadInt32();

            stillCompressedSprite.dataWidth = ((stillCompressedSprite.width + 3) >> 2) << 2; //Round up to the nearest multiple of 4. That's how wide the image really is according to the compressed data.
            if (stillCompressedSprite.fType == 0) stillCompressedSprite.dataWidth = stillCompressedSprite.width; //Type 0 sprites apparently don't need any fixing-up

            stillCompressedSprite.subimages = 1; //Default of 1; if fType is 3 or 5, it may change
            byte term = 0; //Always expected to be 01 (hard-coded in CIsland.exe)
            stillCompressedSprite.rowBeginnings = new int[0];
            stillCompressedSprite.animDataList = new AnimData[0];

            if (stillCompressedSprite.fType == 3 || stillCompressedSprite.fType == 5)
            {
                stillCompressedSprite.subimages = br.ReadInt32();
                stillCompressedSprite.animDataList = new AnimData[stillCompressedSprite.subimages];

                for (int y = 0; y < stillCompressedSprite.subimages; y++)
                {
                    stillCompressedSprite.animDataList[y].unknown1 = br.ReadByte();
                    stillCompressedSprite.animDataList[y].unknown2 = br.ReadByte();
                    stillCompressedSprite.animDataList[y].unknown3 = br.ReadByte();
                    stillCompressedSprite.animDataList[y].unknown4 = br.ReadByte();
                    //TODO: See dilocomp.spr as example. "111 111 105 100" is the integral values of the first one. Second is "239 159 108 100". In fact, they all end with "105" or "108" followed by 100. The first numbers all end with F--6F, 7F, 8F, 9F, AF, BF, CF, DF, EF, FF (none are less than 6F). Anyway, these are four-byte, but not ints.
                    //The second numbers may be 6F, 9F, A0-A6, AF, B0-B6, BF, C0-C5, EF, or F0-F4... That & 0x0F may be frame number within the sequence or F for "final" or something.
                    //It looks like all the bytes are frame properties of some variety, at least.
                }
            }

            //stage1Size is the buffer size needed to accomplish the first decompression stage 1. It's width * height * tile count in the case of fType 5 Sprites (tiles) because they have no stage 2.
            stillCompressedSprite.stage1Size = br.ReadInt32();

            if (stillCompressedSprite.fType == 0 || stillCompressedSprite.fType == 3)
            {
                stillCompressedSprite.rowBeginnings = new int[stillCompressedSprite.height * stillCompressedSprite.subimages + 1];
                for (int x = 0; x < stillCompressedSprite.height * stillCompressedSprite.subimages; x++)
                {
                    stillCompressedSprite.rowBeginnings[x] = br.ReadInt32();
                }

                //May or may not be necessary (if not, don't need the +1 on the height*subimages definition either)
                stillCompressedSprite.rowBeginnings[stillCompressedSprite.height * stillCompressedSprite.subimages] = stillCompressedSprite.stage1Size;
            }

            term = br.ReadByte();
            if (term != 01) System.Diagnostics.Debugger.Break(); //Note: Upon inspection of the disassembly, 01 is a hard-coded terminator.

            //Copy the rest of the data to the imageData array (I'm including 'term' in case it IS part of the image data)
            //Image data is actually in blocks of [[4-byte value, with at least 13 bits of it being block length, but it never exceeds 0x1000][image data which is that many bytes]]
            stillCompressedSprite.imageBlocks = new byte[0][];
            int tl = 0; //tl is just an easier way to refer to imageBlocks.Length - 1
            try
            {
                while (true)
                {
                    Array.Resize(ref stillCompressedSprite.imageBlocks, tl + 1); //Add another block
                    stillCompressedSprite.imageBlocks[tl] = br.ReadBytes(br.ReadInt32()); //The next four bytes are the length of this block
                    if (stillCompressedSprite.imageBlocks[tl].Length != 0x1000) break; //All blocks are 0x1000 except the last block in the file, though it may also be 0x1000.
                    tl++;
                }
            }
            catch (EndOfStreamException) { } //Potentially letting it error out, but it should break after the first block whose length isn't 0x1000

            return 0;
        }

        //Outputs the results of this stage of decompression to outputBuffer.
        //Outputs to "subimages" if fType is 2 or 5. Expects loadedPalette to not be null in those cases.
        public static int decompressSpriteStage1(Sprite stillCompressedSprite, ref byte[] outputBuffer, int[] loadedPalette, ref Bitmap[] subimages)
        {
            outputBuffer = new byte[stillCompressedSprite.stage1Size];
            int outputOffset = 0; //Offset in outputBuffer to write at next
            int nextReferenceFlagsOffset = 0;
            byte refFlagsIndex = 0; //Number of 'units' (1-byte literal or part of a RLESequence, or 2-byte reference) read since the last ReferenceFlags
            byte currentReferenceFlags = 0; //The ReferenceFlags determines which of the 8 following units are 1-byte literals or 2-byte references.

            for (int x = 0; x < stillCompressedSprite.imageBlocks.Length; x++)
            {
                //This code occurs on a per-block basis
                nextReferenceFlagsOffset = 0;
                int y = 0; //Start at input offset zero, but let it advance based on what data you're grabbing. (y is identified as "offset" in my pseudocode)
                while (y < stillCompressedSprite.imageBlocks[x].Length)
                {
                    if (y == nextReferenceFlagsOffset)
                    {
                        //The ReferenceFlags is a bit mask determining which of the next 8 'units' are literals (1 byte) or references (2 bytes). Count the 1-bits, and add that plus 9 to nextReferenceFlagsOffset.
                        currentReferenceFlags = stillCompressedSprite.imageBlocks[x][y];

                        nextReferenceFlagsOffset = y + 9;
                        //This looks silly, but how many better ways are there to count the number of 'on' bits in a single byte?
                        if ((currentReferenceFlags & 0x1) != 0) nextReferenceFlagsOffset++;
                        if ((currentReferenceFlags & 0x2) != 0) nextReferenceFlagsOffset++;
                        if ((currentReferenceFlags & 0x4) != 0) nextReferenceFlagsOffset++;
                        if ((currentReferenceFlags & 0x8) != 0) nextReferenceFlagsOffset++;
                        if ((currentReferenceFlags & 0x10) != 0) nextReferenceFlagsOffset++;
                        if ((currentReferenceFlags & 0x20) != 0) nextReferenceFlagsOffset++;
                        if ((currentReferenceFlags & 0x40) != 0) nextReferenceFlagsOffset++;
                        if ((currentReferenceFlags & 0x80) != 0) nextReferenceFlagsOffset++;

                        //Decipher following units based on currentReferenceFlags.
                        refFlagsIndex = 1; //Allow the units to be deciphered properly. refFlagsIndex is bitshifted over time to compare with currentReferenceFlags's bits more easily.

                        y++;
                    }
                    //I discovered that RLESequence are taken care of on a separate pass from the Refs and ReferenceFlagss.
                    else
                    {
                        if ((currentReferenceFlags & refFlagsIndex) == 0) //If the currentReferenceFlags bit for this refFlagsIndex is 0, it's a literal.
                        {
                            //Output the current byte and advance the offset (aka. y) and outputOffset
                            outputBuffer[outputOffset++] = stillCompressedSprite.imageBlocks[x][y];
                            y++;
                        }
                        else //If the currentReferenceFlags bit for this refFlagsIndex is 1, this is a 2-byte reference.
                        {
                            //I checked the assembly and noticed that these references are actually really simple--exactly as I wrote them here.

                            //Go back this far and copy bytes
                            int subtractCount = (((stillCompressedSprite.imageBlocks[x][y + 1] & 0x0F) << 8) | stillCompressedSprite.imageBlocks[x][y]) + 1;
                            int origin = outputOffset - subtractCount;

                            for (int z = 0; z < ((stillCompressedSprite.imageBlocks[x][y + 1] & 0xF0) >> 4) + 3; z++) //((stillCompressedSprite.imageBlocks[x][y + 1] & 0xF0) >> 4) + 3 is the count of bytes to copy (so at least 3 or up to 18 bytes can be copied at once).
                            {
                                outputBuffer[outputOffset++] = outputBuffer[origin + z];
                            }
                            y += 2;
                        }

                        refFlagsIndex <<= 1; //Bitshift refFlagsIndex instead of adding, so we can compare it to currentReferenceFlags more easily.
                    }

                    Application.DoEvents();

                }

            }
            if (outputBuffer.Length != outputOffset) System.Diagnostics.Debugger.Break();

            //fTypes 2 and 5 are done decompressing. 0 and 3 still need to be put through stage 2.
            if (stillCompressedSprite.fType != 2 && stillCompressedSprite.fType != 5) return 0;

            //Draw bitmaps
            //TODO: Eventually investigate faster way of loading. I'm pretty sure Chaos Island used GDI32 itself, but I don't know how it accounted for team color and shadow.
            try
            {
                outputOffset = 0;
                subimages = new Bitmap[stillCompressedSprite.subimages];
                for (int x = 0; x < stillCompressedSprite.subimages; x++)
                {
                    subimages[x] = new Bitmap(stillCompressedSprite.width, stillCompressedSprite.height);
                    for (int y = 0; y < stillCompressedSprite.dataWidth * stillCompressedSprite.height; y++)
                    {
                        //Don't attempt to draw if it's outside the bitmap's bounds as described by the sprite file header
                        if (y % stillCompressedSprite.dataWidth < stillCompressedSprite.width)
                        {
                            subimages[x].SetPixel(y % stillCompressedSprite.dataWidth, y / stillCompressedSprite.dataWidth, Color.FromArgb(loadedPalette[outputBuffer[outputOffset]]));
                        }
                        outputOffset++;
                    }
                }
            }
            catch { }

            return 0;
        }

        //Decode RLESequence (RLE) and rowBeginnings (basically another RLE method) for fTypes of 0 and 3.
        //Expects loadedPalette to not be null.
        public static int decompressSpriteStage2(Sprite stage1DecompressedSprite, byte[] inputBuffer, int[] loadedPalette, ref Bitmap[] subimages)
        {
            if (stage1DecompressedSprite.fType == 2 || stage1DecompressedSprite.fType == 5) return 0; //Already fully decompressed

            int inputOffset;
            byte[] outputBuffer = new byte[stage1DecompressedSprite.dataWidth * stage1DecompressedSprite.height * stage1DecompressedSprite.subimages];
            int outputOffset = 0; //Offset in outputBuffer
            int nextRLESequenceInputOffset = 0;
            int specialPixelCount = 0;

            //RLESequence decoding: The top two bits may be 00 (transparency), 01 (end-of-series), 10 (team color), or 11 (shadow)
            for (inputOffset = 0; inputOffset < inputBuffer.Length; inputOffset++)
            {
                //When you reach a rowBeginnings value, output blank pixels, then check if outputBuffer % stage1DecompressedSprite.width > 0
                for (int z = 0; z < stage1DecompressedSprite.rowBeginnings.Length; z++)
                {
                    if (inputOffset == stage1DecompressedSprite.rowBeginnings[z])
                    {
                        //Now, this rowBeginnings entry must be the start of a new row of pixels. The only way I can think of to deal with that, for now, is to insert 00s to fill up the current row.
                        //Doesn't depend on the sprite width as the header claims it to be. Depends on dataWidth, which is the width rounded up to a multiple of 4.
                        for (int q = outputOffset % stage1DecompressedSprite.dataWidth; q > 0 && q <= stage1DecompressedSprite.dataWidth; q++) //If outputOffset % stage1DecompressedSprite.width == 0, we don't need to output any.
                        {
                            outputBuffer[outputOffset++] = 0x00; //TODO: Need transparency, not palette index 0.
                        }
                        break;
                    }
                } //End rowBeginnings check

                if (inputOffset >= nextRLESequenceInputOffset)
                {
                    if ((inputBuffer[inputOffset] & 0xC0) == 0x40) //Found a terminator for the active RLESequence
                    {
                        nextRLESequenceInputOffset = inputOffset + (inputBuffer[inputOffset] & 0x3F) + 1;
                    }
                    else
                    {
                        //Immediately output the pixels represented by this byte since the 0x40 and 0x80 bits dictate different colors.
                        specialPixelCount = (inputBuffer[inputOffset] & 0x3F);
                        while (specialPixelCount > 0)
                        {
                            if ((inputBuffer[inputOffset] & 0xC0) == 0x00) outputBuffer[outputOffset++] = 0x00; //0x00 is transparency (which I'm making black for now)
                            else if ((inputBuffer[inputOffset] & 0xC0) == 0x80) outputBuffer[outputOffset++] = 0xC0; //If 0x80 is set and 0x40 is not, it's team color. I'm making it hot pink for now (in the "ms" palette).
                            else outputBuffer[outputOffset++] = 0xDF; //If 0x80 and 0x40 are both set, it's shadow coloring. I'm making it bright yellow for now (in the "ms" palette).

                            specialPixelCount--;
                        }
                    }
                }
                else
                {
                    outputBuffer[outputOffset++] = inputBuffer[inputOffset]; //Increment outputOffset, and copy literals to outputBuffer
                }

                Application.DoEvents();
            }

            //Draw bitmaps (note: this does not properly account for transparency, shadows, and team colors; it is merely intended for depicting the decompressed image.)
            try
            {
                outputOffset = 0;
                subimages = new Bitmap[stage1DecompressedSprite.subimages];
                for (int x = 0; x < stage1DecompressedSprite.subimages; x++)
                {
                    subimages[x] = new Bitmap(stage1DecompressedSprite.width, stage1DecompressedSprite.height);
                    for (int y = 0; y < stage1DecompressedSprite.dataWidth * stage1DecompressedSprite.height; y++)
                    {
                        //Don't attempt to draw if it's outside the bitmap's bounds as described by the sprite file header
                        if (y % stage1DecompressedSprite.dataWidth < stage1DecompressedSprite.width)
                        {
                            subimages[x].SetPixel(y % stage1DecompressedSprite.dataWidth, y / stage1DecompressedSprite.dataWidth, Color.FromArgb(loadedPalette[outputBuffer[outputOffset]]));
                        }
                        outputOffset++;
                    }
                }
            }
            catch { }

            return 0;
        }



    }
}
