using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ChaosIslandHacking
{
    public partial class Form3 : Form
    {
        ChaosLevel[] openMaps = new ChaosLevel[0];
        int cm; //Current map
        Bitmap[] activeTileset = new Bitmap[0];
        int[] activePalette = new int[0];
        Bitmap[] activeBackground = new Bitmap[0]; //Shouldn't have more than 1 entry, but ChaosImage expects an array.
        int paletteSelection = -1; //Currently selected palette entry, if any (-1 if not)
        int lastX = -1, lastY = -1; //The tile that was last painted on, to be cleared when the button is released

        public Form3()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Note: there are totally 44 of these! Even though the game only has 13 or so playable levels!
            openFileDialog1.FileName = "*-pre.LDF";
            openFileDialog1.Filter = "Map pre.LDF files|*-pre.ldf|All files|*.*";
            DialogResult dres = openFileDialog1.ShowDialog();
            if (dres == DialogResult.OK)
            {
                int newMap = openMaps.Length;
                Array.Resize(ref openMaps, openMaps.Length + 1);
                openMaps[newMap] = new ChaosLevel();
                openMaps[newMap].readMapPreLDF(openFileDialog1.FileName);
                openMaps[newMap].readMapSetLDF();
                openMaps[newMap].readMapEndLDF();
                switchMaps(newMap);
            }
        }

        private void picMap_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (openMaps.Length == 0) return;
                e.Graphics.TranslateTransform(-scrollMapX.Value, -scrollMapY.Value);
                if (miViewTerrain.Checked)
                {
                    if (activeBackground[0] != null) e.Graphics.DrawImageUnscaled(activeBackground[0], 0, 0);

                    if (openMaps[cm].useTileset)
                    {
                        //Draw the tile map
                        for (int y = 0; y < openMaps[cm].mapHeight; y++)
                        {
                            for (int x = 0; x < openMaps[cm].mapWidth; x++)
                            {
                                //Don't flip if tilesFlip is 0x00, flip horizontal if tilesFlip & 0x01, flip vertical if tilesFlip & 0x02
                                if (openMaps[cm].tilesFlip[x][y] == 0x00)
                                {
                                    e.Graphics.DrawImageUnscaled(activeTileset[openMaps[cm].tiles[x][y]], 32 * x, 32 * y);
                                }
                                else if (openMaps[cm].tilesFlip[x][y] == 0x01)
                                {
                                    //Flip on X
                                    e.Graphics.DrawImage(activeTileset[openMaps[cm].tiles[x][y]], 32 * (x + 1), 32 * y, -32, 32);
                                }
                                else if (openMaps[cm].tilesFlip[x][y] == 0x02)
                                {
                                    //Flip on Y
                                    e.Graphics.DrawImage(activeTileset[openMaps[cm].tiles[x][y]], 32 * x, 32 * (y + 1), 32, -32);
                                }
                                else
                                {
                                    //Double flipped
                                    e.Graphics.DrawImage(activeTileset[openMaps[cm].tiles[x][y]], 32 * (x + 1), 32 * (y + 1), -32, -32);
                                }
                            }
                        }
                    }
                }

                if (miViewUnits.Checked)
                {
                    //TODO
                    for (int x = 0; x < openMaps[cm].units.Length; x++)
                    {
                        //TODO: Draw units. How do I store their internal names and sprite filenames?
                        //e.Graphics.DrawImageUnscaled(getUnitSprite(openMaps[cm].units[x].name), openMaps[cm].units[x].x * 32, openMaps[cm].units[x].y * 32);
                    }
                }

                if (miViewTerrainObj.Checked)
                {
                    //TODO
                    for (int x = 0; x < openMaps[cm].levelObjects.Length; x++)
                    {
                        //TODO: Draw level objects. How do I store their internal names and sprite filenames?
                        //e.Graphics.DrawImageUnscaled(getLevelObjectSprite(openMaps[cm].levelObjects[x].name), openMaps[cm].levelObjects[x].x * 32, openMaps[cm].levelObjects[x].y * 32);
                    }
                }

                if (miViewRes.Checked)
                {
                    Brush brReservedLoc = new SolidBrush(Color.FromArgb(70, Color.BlueViolet));
                    //I don't understand these yet, so I don't know how I want to draw them.
                    for (int x = 0; x < openMaps[cm].reservedLocs.Length; x++)
                    {
                        e.Graphics.FillEllipse(brReservedLoc, openMaps[cm].reservedLocs[x].x * 32, openMaps[cm].reservedLocs[x].y * 32, 32, 32);
                    }
                }

                if (miViewAIMarkers.Checked)
                {
                    //Not sure how I want to draw these; they can be AIRDROP | DEFEND | GENSITE | GRAZE | HANGOUT | TRAIL | PROXSENSOR, and some are specific to unit types (like AllGatherers or just Compy). I'm thinking pictures would be best.
                    for (int x = 0; x < openMaps[cm].aiMarkers.Length; x++)
                    {
                        //Draw a particular image depending on the type
                        switch (openMaps[cm].aiMarkers[x].type)
                        {
                            case "AIRDROP":
                                e.Graphics.DrawImageUnscaled(Resource1.Airdrop, openMaps[cm].aiMarkers[x].x * 32, openMaps[cm].aiMarkers[x].y * 32);
                                break;
                            case "DEFEND":
                                e.Graphics.DrawImageUnscaled(Resource1.Defend, openMaps[cm].aiMarkers[x].x * 32, openMaps[cm].aiMarkers[x].y * 32);
                                break;
                            case "GENSITE":
                                e.Graphics.DrawImageUnscaled(Resource1.Gensite, openMaps[cm].aiMarkers[x].x * 32, openMaps[cm].aiMarkers[x].y * 32);
                                break;
                            case "GRAZE":
                                e.Graphics.DrawImageUnscaled(Resource1.Graze, openMaps[cm].aiMarkers[x].x * 32, openMaps[cm].aiMarkers[x].y * 32);
                                break;
                            case "HANGOUT":
                                e.Graphics.DrawImageUnscaled(Resource1.Hangout, openMaps[cm].aiMarkers[x].x * 32, openMaps[cm].aiMarkers[x].y * 32);
                                break;
                            case "TRAIL":
                                e.Graphics.DrawImageUnscaled(Resource1.Trail, openMaps[cm].aiMarkers[x].x * 32, openMaps[cm].aiMarkers[x].y * 32);
                                break;
                            case "PROXSENSOR":
                                e.Graphics.DrawImageUnscaled(Resource1.Proxsensor, openMaps[cm].aiMarkers[x].x * 32, openMaps[cm].aiMarkers[x].y * 32);
                                break;
                        }

                        //Draw radius for those with radius
                        if (openMaps[cm].aiMarkers[x].radius > 0)
                        {
                            e.Graphics.DrawEllipse(Pens.Yellow, (openMaps[cm].aiMarkers[x].x - openMaps[cm].aiMarkers[x].radius) * 32 + 16, (openMaps[cm].aiMarkers[x].y - openMaps[cm].aiMarkers[x].radius) * 32 + 16,
                                openMaps[cm].aiMarkers[x].radius * 64, openMaps[cm].aiMarkers[x].radius * 64);
                        }
                        else if (openMaps[cm].aiMarkers[x].radius == -1)
                        {
                            //Draw an "X" because the radius is infinite
                            e.Graphics.DrawLine(Pens.Yellow, openMaps[cm].aiMarkers[x].x * 32 - 16, openMaps[cm].aiMarkers[x].y * 32 - 16,
                                openMaps[cm].aiMarkers[x].x * 32 + 48, openMaps[cm].aiMarkers[x].y * 32 + 48);
                        }
                    }
                }

                if (miViewPassability.Checked)
                {
                    Brush brBuildable = new SolidBrush(Color.FromArgb(50, Color.Green));
                    Brush brUnbuildable = new SolidBrush(Color.FromArgb(50, Color.Gray));
                    Brush brUnwalkable = new SolidBrush(Color.FromArgb(50, Color.Red));

                    for (int y = 0; y < openMaps[cm].mapHeight; y++)
                    {
                        for (int x = 0; x < openMaps[cm].mapWidth; x++)
                        {
                            if (openMaps[cm].passabilityMask[x][y] == 4) e.Graphics.FillRectangle(brUnwalkable, x * 32, y * 32, 32, 32);
                            else if (openMaps[cm].passabilityMask[x][y] == 5) e.Graphics.FillRectangle(brBuildable, x * 32, y * 32, 32, 32);
                            else if (openMaps[cm].passabilityMask[x][y] == 6) e.Graphics.FillRectangle(brUnbuildable, x * 32, y * 32, 32, 32);
                            else System.Diagnostics.Debugger.Break(); //TODO: Possible other options
                        }
                    }
                }


                int newMax = openMaps[cm].mapWidth * 32 - picMap.Width + 96; //Don't know why Microsoft thought it'd be brilliant to make LargeChange affect the user-reachable maximum
                if (scrollMapX.Maximum != newMax)
                {
                    if (scrollMapX.Value > newMax) scrollMapX.Value = newMax;
                    scrollMapX.Maximum = newMax;
                    scrollMapX.LargeChange = 96;
                    scrollMapX.SmallChange = 16;
                    scrollMapX.Visible = (newMax != 0);
                }

                newMax = openMaps[cm].mapHeight * 32 - picMap.Height + 96;
                if (scrollMapY.Maximum != newMax)
                {
                    if (scrollMapY.Value > newMax) scrollMapY.Value = newMax;
                    scrollMapY.Maximum = newMax;
                    scrollMapY.LargeChange = 96;
                    scrollMapY.SmallChange = 16;
                    scrollMapY.Visible = (newMax != 0);
                }
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.Print(err.Message);
            }
        }

        private void picPalette_Paint(object sender, PaintEventArgs e)
        {
            //TODO: Palettes other than terrain

            int newMax = 0; //scrollPalette's new maximum value
            int tilesWide = picPalette.Width / 32;
            e.Graphics.TranslateTransform(0, -scrollPalette.Value);

            for (int x = 0; x < activeTileset.Length; x++)
            {
                e.Graphics.DrawImageUnscaled(activeTileset[x], (x % tilesWide) * 32, (x / tilesWide) * 32);
            }

            //Draw box around selected tile
            if (paletteSelection != -1) e.Graphics.DrawRectangle(Pens.Red, (paletteSelection % tilesWide) * 32, (paletteSelection / tilesWide) * 32, 32, 32);

            //Calculate new max value for scrollPalette in case it changed
            //(activeTileset.Length + tilesWide - 1) / tilesWide is the number of rows needed.
            newMax = (activeTileset.Length + tilesWide - 1) / tilesWide * 32 - picPalette.Height + 96;
            if (newMax < 0) newMax = 0;
            if (scrollPalette.Maximum != newMax)
            {
                if (scrollPalette.Value > newMax) scrollPalette.Value = newMax;
                scrollPalette.Maximum = newMax;
                scrollPalette.Visible = (scrollPalette.Maximum != 0); //Make visible or invisible
                scrollPalette.LargeChange = 96;
                scrollPalette.SmallChange = 16;
            }
        }

        private void switchMaps(int newSelection)
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            //Load the palette
            int res = ChaosImage.loadPalette(Path.Combine(Path.GetDirectoryName(openMaps[newSelection].mapName), openMaps[newSelection].paletteName), ref activePalette);
            if (res != 0)
            {
                MessageBox.Show("The map's chosen palette could not be loaded.");
            }

            //Load the tileset
            ChaosImage.Sprite tempSprite = new ChaosImage.Sprite();
            byte[] outputBuffer = new byte[0];
            if (openMaps[newSelection].backgroundTileset != "")
            {
                res = ChaosImage.loadSprite(Path.Combine(Path.GetDirectoryName(openMaps[newSelection].mapName), openMaps[newSelection].backgroundTileset), ref tempSprite);
                if (res != 0)
                {
                    MessageBox.Show("The map's tileset could not be loaded.");
                }
                res = ChaosImage.decompressSpriteStage1(tempSprite, ref outputBuffer, activePalette, ref activeTileset);
                if (res != 0)
                {
                    MessageBox.Show("The map's tileset could not be decompressed (stage 1).");
                }
                //I don't think any of the tilesets have a stage 2 decompression, but I'll leave this here anyway. Can't hurt.
                res = ChaosImage.decompressSpriteStage2(tempSprite, outputBuffer, activePalette, ref activeTileset);
                if (res != 0)
                {
                    MessageBox.Show("The map's tileset could not be decompressed (stage 2).");
                }
            }

            //Load the background
            if (!openMaps[newSelection].useTileset && openMaps[newSelection].background != "")
            {
                res = ChaosImage.loadSprite(Path.Combine(Path.GetDirectoryName(openMaps[newSelection].mapName), openMaps[newSelection].background), ref tempSprite);
                if (res != 0)
                {
                    MessageBox.Show("The map's background could not be loaded.");
                }
                res = ChaosImage.decompressSpriteStage1(tempSprite, ref outputBuffer, activePalette, ref activeBackground);
                if (res != 0)
                {
                    MessageBox.Show("The map's background could not be decompressed (stage 1).");
                }
                res = ChaosImage.decompressSpriteStage2(tempSprite, outputBuffer, activePalette, ref activeBackground);
                if (res != 0)
                {
                    MessageBox.Show("The map's background could not be decompressed (stage 2).");
                }
            }

            cm = newSelection; //Update the loaded map index
            picPalette.Refresh();
            picMap.Refresh();
            Cursor.Current = Cursors.Default;
        }

        private void scrollPalette_ValueChanged(object sender, EventArgs e)
        {
            picPalette.Invalidate();
        }

        private void scrollMapY_Scroll(object sender, ScrollEventArgs e)
        {
            picMap.Invalidate();
        }

        private void scrollMapX_Scroll(object sender, ScrollEventArgs e)
        {
            picMap.Invalidate();
        }

        private void viewMenuSubitem_Click(object sender, EventArgs e)
        {
            picMap.Invalidate();
        }

        private void picPalette_MouseUp(object sender, MouseEventArgs e)
        {
            //TODO: May need to change this code up (definitely have to change activePalette.Length references) for palettes other than terrain
            int tilesWide = picPalette.Width / 32;
            if (activePalette.Length == 0 || e.Y >= picPalette.Height || e.Y < 0 || e.X < 0 || e.X >= tilesWide * 32) return; //Do nothing if the mouse button was released outside the palette area

            picPalette.Invalidate(new Rectangle((paletteSelection % tilesWide) * 32, (paletteSelection / tilesWide) * 32 - scrollPalette.Value, 33, 33)); //Redraw the previously selected entry, if there was one

            paletteSelection = ((e.Y + scrollPalette.Value) / 32) * tilesWide + e.X / 32; //Select a palette entry
            if (paletteSelection < 0 || paletteSelection > activePalette.Length - 1) paletteSelection = -1; //Keep in range

            picPalette.Invalidate(new Rectangle((paletteSelection % tilesWide) * 32, (paletteSelection / tilesWide) * 32 - scrollPalette.Value, 33, 33)); //Redraw the newly selected entry
        }

        private void picMap_MouseUp(object sender, MouseEventArgs e)
        {
            //Refire Mouse Move event in case the user didn't move the mouse at all but attempted to paint a tile. (May want to move that to MouseDown.) Also clear lastX (it isn't necessary to clear LastY as well).
            picMap_MouseMove(sender, e);
            lastX = -1;
            //TODO: Append to Undo list
        }

        private void picMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int tempX = e.X + scrollMapX.Value, tempY = e.Y + scrollMapY.Value;
                if (cm == -1 || tempX < 0 || tempY < 0 || tempX >= openMaps[cm].mapWidth * 32 || tempY >= openMaps[cm].mapHeight * 32 || paletteSelection == -1) return; //Stay in bounds
                if (terrainToolStripMenuItem.Checked) //Can only work with terrain when terrain layer is selected
                {
                    if (lastX == tempX / 32 && lastY == tempY / 32) return; //Also make sure the last-clicked tile (cleared when the mouse was last released) was not this one
                    openMaps[cm].tiles[tempX / 32][tempY / 32] = paletteSelection; //Update the tile
                    lastX = tempX / 32;
                    lastY = tempY / 32;
                    picMap.Invalidate(new Rectangle((e.X / 32) * 32, (e.Y / 32) * 32, 32, 32)); //Redraw only what's necessary
                    //TODO: Stuff for flipping, copy and paste, etc.
                    //For flipping, shift may be H and control may be V, and you could check the states of those on keydown and keyup events and draw the flipped tiles in the palette box.
                } //TODO: Other layers
            }


        }

        private void layerToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //Uncheck all the items except the one the user just clicked
            for (int x = 0; x < layerToolStripMenuItem.DropDownItems.Count; x++)
            {
                ((ToolStripMenuItem)layerToolStripMenuItem.DropDownItems[x]).Checked = (layerToolStripMenuItem.DropDownItems[x] == e.ClickedItem);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
