using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ChaosIslandHacking
{
    class ChaosLevel
    {
        //TODO: Note that level name-file associations are made in levelloadinfo.ldf
        //TODO: Also note that levelobjanim.ldf tells the game how to animate LEVELOBJ like trees, bridges, and nests
        //The files come in -pre, -set, and -end varieties. Just load -pre ones, because they reference -set and -end ones.
        //*****BEGIN -pre DATA*****
        //I wrote whether the various fields had an extension or not, but it seems to depend on which map you load, so I'll go ahead and check them with Path.HasExtension.
        public int initialPoints; //Initial points for purchasing gatherers
        public int initialSupplies;
        public int resupplyThresh; //"Resupply threshold value" (not sure what this means yet)
        public int minGatherers; //Can't start level without at least this many
        public int maxGatherers; //Can't select more than this many before starting the level (not sure if it affects getting more gatherers mid-game)

        public string recruitAudio; //File to play during base recruit screen
        public string levelAudioSet; //Level audio set name
        public string cutSceneName; //TODO: Unknown if requires extension
        public string missionBriefingName; //TODO: Unknown if requires extension
        public string levelAI; //The AI to use for this level (no ext--it's not a file, I think, anyway)
        public string nextLevel; //Name of next level to play (no ext)
        public bool useTileset; //In file, "0" or "1". One map file says "use tileset only," but all the others just say "use tileset." It appears that the tileset is not, in fact, used...just about ever. :P
        public string backgroundTileset; //Background tileset filename (no ext)
        public string tileMapFile; //Level description filename (including ext)

        public byte mapWidth; //28, 46, 56, 64... Looks like any even number should be fine, up to 64.
        public byte mapHeight;
        public string background; //Level background (including ext)
        public string paletteName; //Filename of palette (including ext)
        public string objectives; //Name of objectives sprite (no ext)
        public string objectivesAudio; //(including ext)

        //6 availability bools
        public bool[] available; //Eddie, Malcolm, Nick, Sarah, Kelly, Graduate Student

        //8 initial egg bytes
        public byte[] initialEggs; //TRex, Triceratops, Steg, Dilo, Para, Pachy, Raptor, Compy

        //14 "can build X" bools
        public bool[] techTree; //Base Camp 1, 2, 3, Tent, Shelter, Hardened Shelter, Artificial Nest, Incubator, Super Incubator (not implemented?), Tool Shed, Electronics Shed, Science Lab, High Hide, Fire Barrier

        //20 "audio track available" bools
        public bool[] audioTracks; //1 through 20

        //Named as they are in the -pre file
        public byte m_bKillGatherer; //Hunter Aggression against Gatherers
        public byte m_bHuntDinos; //Hunter Desire to Hunt Dinos
        public byte m_bHumanContact; //Rogue dino likelihood to attack/flee humans

        //8 of each of these, all the same as initialEggs: TRex, Triceratops, Steg (named "Stegosaur" as a unit), Dilo, Para, Pachy, Raptor, Compy
        public byte[] rogueDinoResupplyThreshold = new byte[8];
        public int[] rogueDinoCreationMax = new int[8];
        public int[] rogueDinoCreationInitialDelay = new int[8];
        public int[] rogueDinoCreationInterval = new int[8];

        //5 of each of these: Scout, Rifleman, Jeep, Snagger, Tank
        public byte[] hunterResupplyThreshold = new byte[5];
        public int[] hunterCreationMax = new int[5];
        public int[] hunterCreationInitialDelay = new int[5];
        public int[] hunterCreationInterval = new int[5];

        //And 8 of each of these again
        public byte[] hunterDinoResupplyThreshold = new byte[8];
        public int[] hunterDinoCreationMax = new int[8];
        public int[] hunterDinoCreationInitialDelay = new int[8];
        public int[] hunterDinoCreationInterval = new int[8];

        //*****END OF -pre DATA*****
        //Only one thing in the -set data:
        public int[][] tiles; //Tile map from tileMapFile. Tile IDs can be greater than 255, and in the "shelly" maps, they're all -1 (who knows what this does?).
        public byte[][] tilesFlip; //Two flags. 0x01 is horizontal flip, 0x02 is vertical flip.

        //*****BEGIN -end DATA*****
        public byte initialViewportX; //Where the screen is looking when you start the mission
        public byte initialViewportY;

        public byte[][] passabilityMask; //It looks like 4 is unwalkable, 5 is buildable, and 6 is walkable but unbuildable

        //LEVELOBJ [TERRAIN | ???] "name" <HP, -1 probably means max> <X> <Y>
        public LevelObj[] levelObjects; //"Terrain" type objects include trees

        //RESERVEDLOC <InitialFacing> <X> <Y>
        public ReservedLoc[] reservedLocs; //Beats me

        //UNIT <Special AI 0|1, seems optional; only 1 for player-owned stuff, I think> [BTREX | GATHERER | DINO | SUPPLIES | TNT | EGGS | BUILDING | HUNTER | ???] "name, which is an item like carry2 in the case of SUPPLIES" <Rogue, 0|1, if it's a dino> <hp, absent for BTREX and TNT> <InitialFacing, or ID if it's a building, absent for BTREX> <X> <Y>
        public Unit[] units;

        //AIMARKER [AIRDROP | DEFEND | GENSITE | GRAZE | HANGOUT | TRAIL | PROXSENSOR] "unit" <radius> <message, but not for PROXSENSOR or AIRDROP> <ID, often -1> <X> <Y>
        //GENSITE only has unit, x, y
        public AIMarker[] aiMarkers; //Unit can be things like "All" or "AllGatherers" "AllHunters" "AllDinos" or "Compy"

        public struct LevelObj
        {
            public string type; //Probably won't be keeping that as a string forever, but "TERRAIN" is one example
            public string name; //Name of object specifically, like PalmTree, PalmTree2, or CrazyPalm
            public int hp; //-1, 20, 30, 70 are some examples. -1 is probably "max", NOT invincible.
            public int x;
            public int y;
        }

        public struct ReservedLoc
        {
            public int initialFacing;
            public int x;
            public int y;
        }

        public struct Unit
        {
            public bool specialAI;
            public string type;
            public string name;
            public bool rogue;
            public int hp;
            public int initialFacing; public int ID; //ID if building, InitialFacing otherwise
            public int x;
            public int y;
        }

        public struct AIMarker
        {
            public string type;
            public string name;
            public int radius;
            public int message;
            public int ID;
            public int x;
            public int y;
        }

        //After all is said and done, the -pre and -end files end with "END"

        //***************************************************
        //Variables for internal program use only (not saved) are below this line.
        public string mapName;

        //TODO: Some sort of Undo list goes here. I'm thinking I want to keep one complete copy (basis) and a list of changes (of their own unique types), and reapply all but the most recent change if the user hits Undo.



        //TODO: LDF files in general could use this, so let's make a LDFReader
        //Return true if it finds a number, false if it finds a string (or end of file).
        public bool findNextToken(string getFrom, ref int nPos)
        {
            while (!((getFrom[nPos] >= '0' && getFrom[nPos] <= '9') || getFrom[nPos] == '-' || getFrom[nPos] == '"' ||
                (getFrom[nPos] >= 'A' && getFrom[nPos] <= 'Z') || (getFrom[nPos] >= 'a' && getFrom[nPos] <= 'z')))
            {
                if (getFrom[nPos] == ';') skipToNextLine(getFrom, ref nPos); //Skip the rest of the line if you hit a comment
                else nPos++;
                if (nPos >= getFrom.Length) return false; //EOF
            }

            if ((getFrom[nPos] >= '0' && getFrom[nPos] <= '9') || getFrom[nPos] == '-') return true; //It's a number
            return false; //It's a string
        }

        //Assumes nPos is already pointing to the first character of the string (if it's a quoted string, point to the quotation mark)
        public string readString(string getFrom, ref int nPos)
        {
            string outStr;
            int startPos;
            if (getFrom[nPos] == '"') //String surrounded in quotation marks
            {
                nPos++; //Skip the starting quotation mark
                startPos = nPos;
                while (getFrom[nPos] != '"') nPos++;
                outStr = getFrom.Substring(startPos, nPos - startPos);
                nPos++; //Skip the closing quotation mark
            }
            else
            { //String without quotation marks
                startPos = nPos;
                while ((getFrom[nPos] >= '0' && getFrom[nPos] <= '9') || (getFrom[nPos] >= 'A' && getFrom[nPos] <= 'Z') ||
                    (getFrom[nPos] >= 'a' && getFrom[nPos] <= 'z'))
                {
                    if (getFrom[nPos] == ';') skipToNextLine(getFrom, ref nPos); //Skip the rest of the line if you hit a comment
                    else nPos++;
                }
                outStr = getFrom.Substring(startPos, nPos - startPos);
            }

            return outStr;
        }

        //Assumes nPos is already pointing to the first character of the numeric token.
        public int readNumber(string getFrom, ref int nPos)
        {
            int startPos = nPos;
            while ((getFrom[nPos] >= '0' && getFrom[nPos] <= '9') || getFrom[nPos] == '-') nPos++;
            return Int32.Parse(getFrom.Substring(startPos, nPos - startPos));
        }

        //Advance to the beginning of the next line of text
        public void skipToNextLine(string getFrom, ref int nPos)
        {
            while (getFrom[nPos++] != '\n') ;
        }

        public bool nextTokenIsEnd(string getFrom, int nPos)
        {
            if (getFrom[nPos] == 'E' && getFrom[nPos + 1] == 'N' && getFrom[nPos + 2] == 'D') return true; //"END" found
            return false;
        }

        //Process: open file. findNextToken. readString or readNumber, whichever you need.
        //If you need to check for an optional number like SpecialAI, check if ((getFrom[nPos] >= '0' && getFrom[nPos] <= '9') || getFrom[nPos] == '-')
        public void readMapPreLDF(string filename)
        {
            string wholeFile = "";
            int nPos = 0;
            mapName = filename;
            wholeFile = File.ReadAllText(filename);

            findNextToken(wholeFile, ref nPos);
            initialPoints = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            initialSupplies = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            resupplyThresh = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            minGatherers = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            maxGatherers = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);

            recruitAudio = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            levelAudioSet = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            cutSceneName = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            missionBriefingName = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            levelAI = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            nextLevel = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            useTileset = (readNumber(wholeFile, ref nPos) == 1); findNextToken(wholeFile, ref nPos);
            backgroundTileset = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            if (backgroundTileset != "" && !Path.HasExtension(backgroundTileset)) backgroundTileset += ".spr";
            tileMapFile = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            if (tileMapFile != "" && !Path.HasExtension(tileMapFile)) tileMapFile += ".ldf";

            mapWidth = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            mapHeight = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            background = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            if (background != "" && !Path.HasExtension(background)) background += ".spr";
            paletteName = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            if (paletteName != "" && !Path.HasExtension(paletteName)) paletteName += ".pal";
            objectives = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            if (objectives != "" && !Path.HasExtension(objectives)) objectives += ".spr";
            objectivesAudio = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);

            //Gatherer available for selection at game start
            available = new bool[6];
            for (int x = 0; x < 6; x++)
            {
                available[x] = (readNumber(wholeFile, ref nPos) == 1); findNextToken(wholeFile, ref nPos); 
            }

            initialEggs = new byte[8];
            for (int x = 0; x < 8; x++)
            {
                initialEggs[x] = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }

            techTree = new bool[14];
            for (int x = 0; x < 14; x++)
            {
                techTree[x] = (readNumber(wholeFile, ref nPos) == 1); findNextToken(wholeFile, ref nPos);
            }

            audioTracks = new bool[20];
            for (int x = 0; x < 20; x++)
            {
                audioTracks[x] = (readNumber(wholeFile, ref nPos) == 1); findNextToken(wholeFile, ref nPos);
            }

            m_bKillGatherer = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            m_bHuntDinos = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            m_bHumanContact = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);

            //Rogue dino params (8 each)
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 8; x++)
            {
                rogueDinoResupplyThreshold[x] = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 8; x++)
            {
                rogueDinoCreationMax[x] = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 8; x++)
            {
                rogueDinoCreationInitialDelay[x] = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 8; x++)
            {
                rogueDinoCreationInterval[x] = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }

            //Hunter params (5 each)
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 5; x++)
            {
                hunterResupplyThreshold[x] = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 5; x++)
            {
                hunterCreationMax[x] = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 5; x++)
            {
                hunterCreationInitialDelay[x] = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 5; x++)
            {
                hunterCreationInterval[x] = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }

            //Hunter dino params (8 each)
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 8; x++)
            {
                hunterDinoResupplyThreshold[x] = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 8; x++)
            {
                hunterDinoCreationMax[x]=readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 8; x++)
            {
                hunterDinoCreationInitialDelay[x]= readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }
            if (nextTokenIsEnd(wholeFile, nPos)) return;
            for (int x = 0; x < 8; x++) 
            {
                hunterDinoCreationInterval[x] = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            }
        }

        public void readMapSetLDF()
        {
            //Load the file (*-set.ldf) that associates the tile images with locations on this map.
            if (tileMapFile != "")
            {
                //tileMapFile comes with extension attached.
                string wholeFile = File.ReadAllText(Path.Combine(Path.GetDirectoryName(mapName), tileMapFile));
                int nPos = 0;
                findNextToken(wholeFile, ref nPos);

                //Read tile indexes
                tiles = new int[mapWidth][];
                tilesFlip = new byte[mapWidth][];
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        if (y == 0)
                        {
                            tiles[x] = new int[mapHeight];
                            tilesFlip[x] = new byte[mapHeight];
                        }
                        tiles[x][y] = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                        //Flip tiles
                        tilesFlip[x][y] = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    }
                }
            }
        }

        public void readMapEndLDF()
        {
            //Load next file (-end)
            int tIdx = 0;
            bool nextTokenIsNumber;
            string wholeFile = File.ReadAllText(mapName.Replace("-pre.ldf", "-end.ldf"));
            int nPos = 0;
            findNextToken(wholeFile, ref nPos);

            initialViewportX = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
            initialViewportY = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);

            //	passabilityMask		byte[][]
            passabilityMask = new byte[mapWidth][];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (y == 0) passabilityMask[x] = new byte[mapHeight];
                    passabilityMask[x][y] = (byte)readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                }
            }

            string nextType = readString(wholeFile, ref nPos); nextTokenIsNumber = findNextToken(wholeFile, ref nPos);
            while (nextType != "END")
            {
                if (nextType == "LEVELOBJ")
                {
                    if (levelObjects == null) levelObjects = new LevelObj[0];
                    tIdx = levelObjects.Length;
                    Array.Resize(ref levelObjects, tIdx + 1);

                    //Have only found type = TERRAIN so far.
                    levelObjects[tIdx].type = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    //Some example names: PalmTree, PalmTree2, CrazyPalm
                    levelObjects[tIdx].name = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    levelObjects[tIdx].hp = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    levelObjects[tIdx].x = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    levelObjects[tIdx].y = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                }
                else if (nextType == "RESERVEDLOC")
                {
                    if (reservedLocs == null) reservedLocs = new ReservedLoc[0];
                    tIdx = reservedLocs.Length;
                    Array.Resize(ref reservedLocs, tIdx + 1);

                    reservedLocs[tIdx].initialFacing = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    reservedLocs[tIdx].x = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    reservedLocs[tIdx].y = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                }
                else if (nextType == "UNIT")
                {
                    if (units == null) units = new Unit[0];
                    tIdx = units.Length;
                    Array.Resize(ref units, tIdx + 1);

                    if (nextTokenIsNumber) //Optional "SpecialAI" (1 = owned by player)
                    {
                        units[tIdx].specialAI = (readNumber(wholeFile, ref nPos) == 1); findNextToken(wholeFile, ref nPos);
                    }
                    //May be BTREX, BUILDING, EGGS, DINO, GATHERER, HUNTER, SUPPLIES, or TNT
                    units[tIdx].type = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);

                    //May be a lot of things, and it's in quotation marks. In the case of SUPPLIES, it's the name of an upgrade (like "carry2") that you get upon first taking from the pile.
                    units[tIdx].name = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);

                    if (units[tIdx].type == "DINO") //Dinosaurs have a "rogue" flag
                    {
                        units[tIdx].rogue = (readNumber(wholeFile, ref nPos) == 1); findNextToken(wholeFile, ref nPos);
                    }
                    if (units[tIdx].type != "BTREX" && units[tIdx].type != "TNT") //Baby T-Rex and TNT don't have HP
                    {
                        units[tIdx].hp = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    }
                    if (units[tIdx].type == "BUILDING") //Buildings have an ID instead of initialFacing
                    {
                        units[tIdx].ID = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    }
                    else if (units[tIdx].type != "BTREX") //Baby T-Rex doesn't have an ID or initialFacing here.
                    {
                        units[tIdx].initialFacing = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    }
                    units[tIdx].x = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    units[tIdx].y = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);

                }
                else if (nextType == "AIMARKER")
                {
                    if (aiMarkers == null) aiMarkers = new AIMarker[0];
                    tIdx = aiMarkers.Length;
                    Array.Resize(ref aiMarkers, tIdx + 1);

                    //May be AIRDROP, DEFEND, GENSITE, GRAZE, HANGOUT, TRAIL, or PROXSENSOR
                    aiMarkers[tIdx].type = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    aiMarkers[tIdx].name = readString(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    if (aiMarkers[tIdx].type != "GENSITE") //GENSITE doesn't have these three fields
                    {
                        aiMarkers[tIdx].radius = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                        if (aiMarkers[tIdx].type != "PROXSENSOR" && aiMarkers[tIdx].type != "AIRDROP") //PROXSENSOR and AIRDROP don't have the message field
                        {
                            aiMarkers[tIdx].message = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                        }
                        aiMarkers[tIdx].ID = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    }
                    aiMarkers[tIdx].x = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                    aiMarkers[tIdx].y = readNumber(wholeFile, ref nPos); findNextToken(wholeFile, ref nPos);
                }

                nextType = readString(wholeFile, ref nPos); nextTokenIsNumber = findNextToken(wholeFile, ref nPos);
            }
        }

    }
}
