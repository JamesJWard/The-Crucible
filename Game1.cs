using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace The_Crucible
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region enum

        enum GameState
        {
            Title,
            Setup,
            Game,
            End
        }
        GameState state = GameState.Title;

        #endregion

        #region states

        #region title
        Texture2D BackgroundTex;
        Rectangle BackgroundRec;

        Texture2D JuryTex;
        Rectangle JuryRec;

        Texture2D hangBarTex;
        Rectangle hangBarRec;

        SpriteFont TitleFont;
        SpriteFont HelpFont;

        float PortWidth;
        float PortHeight;

        bool fadeup = false;
        int fade = 150;

        void loadTitle()
        {
            BackgroundTex = Content.Load<Texture2D>("images/CourtRoom");
            BackgroundRec = new Rectangle(0, 0, (int)(PortWidth), (int)(PortHeight));

            JuryTex = Content.Load<Texture2D>("images/ChairsStrip");
            JuryRec = new Rectangle((int)(PortWidth / 1.6f), (int)(PortHeight / 2.8f), (int)(PortWidth / 3), (int)(PortHeight / 2));

            hangBarTex = Content.Load<Texture2D>("images/HangBar");
            hangBarRec = new Rectangle((int)(PortWidth / 1.5f), (int)(PortHeight / 4.6f), (int)(PortWidth / 10), (int)(PortHeight / 15));

            HelpFont = Content.Load<SpriteFont>("fonts/Help");
            TitleFont = Content.Load<SpriteFont>("fonts/Title");
        }
        void updateTitle()
        {
            KeyboardState keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.A))
            {
                state = GameState.Setup;
            }

            if (fade <= 150) fadeup = true;
            if (fade >= 255) fadeup = false;

            if (fadeup == true) fade += 5;
            else fade -= 5;

        }
        void drawTitle()
        {
            spriteBatch.Begin();
            //background
            spriteBatch.Draw(BackgroundTex, BackgroundRec, Color.White);
            spriteBatch.Draw(hangBarTex, hangBarRec, Color.White);
            spriteBatch.Draw(JuryTex, JuryRec, new Rectangle(0, 0, 126, 139), Color.White);
            //text
            for (int i = 0; i < 5; i++)
            {
                spriteBatch.DrawString(TitleFont, "The Crucible", new Vector2(0 + i, 0 - i), Color.Gray);
            }
            spriteBatch.DrawString(TitleFont, "The Crucible", new Vector2(0, 0), Color.White);

            spriteBatch.DrawString(HelpFont, "Press A to start", new Vector2(100, 100), new Color(fade, fade, fade));
            spriteBatch.End();
        }
        #endregion

        #region Setup
        void updateSetUp()
        {
            numofConvicted = 0;
            numSaved = 0;
            ConLineRec.X = (int)(PortWidth / 2);
            state = GameState.Game;
        }
        #endregion

        #region Game

        #region tex & rec
        Texture2D cheatTex;
        Rectangle cheatRec;

        Texture2D accussedTex;
        Rectangle accussedRec;

        Texture2D judgeTex;
        Rectangle judgeRec;

        Rectangle JuryFrame;
        Rectangle Accussedframe;
        Rectangle JudgeFrame;

        Texture2D barTex;
        Rectangle barRec;

        Texture2D cursorTex;
        Rectangle cursorRec;

        Texture2D bashExploTex;

        SpriteFont bashButton;

        #endregion
        #region mechanical

        bool cheating;
        int numofConvicted;
        int numSaved;

        KeyboardState oldkey;

        /// <summary>
        /// 1 = a, 2 = s, 3 = d.
        /// </summary>
        int buttontobebashed;
        int buttonsteptimer;
        Random rand = new Random();

        int juryframeint;
        int accussedframeint;
        int judgeframeint;

        bool start;
        bool arguing;
        /// <summary>
        /// 0 = !, 1 = CON, 2 = SAV;
        /// </summary>
        int result;

        string KeytobeDisplayed;
        int exploTimer;
        #endregion

        void loadGame()
        {
            cheatTex = Content.Load<Texture2D>("images/AfraidGhost");
            cheatRec = new Rectangle(0,0,(int)(PortWidth / 25), (int)(PortHeight / 20));

            accussedTex = Content.Load<Texture2D>("images/AccussedStrip");
            accussedRec = new Rectangle((int)(0 - PortWidth / 12), (int)(PortHeight / 2.1f), (int)(PortWidth / 12), (int)(PortHeight / 3));

            judgeTex = Content.Load<Texture2D>("images/JudgeStrip");
            judgeRec = new Rectangle((int)(PortWidth / 7), (int)(PortHeight / 7), (int)(PortWidth / 4), (int)(PortHeight / 5));

            barTex = Content.Load<Texture2D>("images/Bar");
            barRec = new Rectangle(0, (int)(PortHeight - (PortHeight / 8)), (int)(PortWidth), (int)(PortHeight / 8));

            cursorTex = Content.Load<Texture2D>("images/Cursor");
            cursorRec = new Rectangle((int)((PortWidth / 2) - ((PortWidth / 10)) / 2), (int)(PortHeight - (PortHeight / 8)), (int)(PortWidth / 10), (int)(PortHeight / 8));

            bashExploTex = Content.Load<Texture2D>("images/bash!");

            bashButton = Content.Load<SpriteFont>("fonts/BashButton");

            JuryFrame = new Rectangle(127, 0, 126, 139);
            Accussedframe = new Rectangle(0, 0, 44, 102);
            JudgeFrame = new Rectangle(0, 0, 109, 64);
        }

        void updateGame()
        {
            KeyboardState keys = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            #region entrance

            if (start == false)
            {
                if (accussedRec.X < (int)(PortWidth / 11))
                {
                    accussedRec.X++;
                    if (accussedframeint > 0) accussedframeint--;
                    else accussedframeint = 30;

                    Accussedframe.Y = 0;
                    if (accussedframeint > 15) Accussedframe.X = 0;
                    if (accussedframeint < 15) Accussedframe.X = 45;
                }
                else
                {
                    Accussedframe.X = 91;
                    start = true;
                    arguing = true;
                }
            }

            #endregion
            #region Game

            if (arguing == true)
            {
                if (judgeframeint > 0) judgeframeint--;
                else judgeframeint = 60;

                if (juryframeint > 0) juryframeint--;
                else juryframeint = 30;

                if (juryframeint > 15) JuryFrame.X = 254;
                if (juryframeint < 15) JuryFrame.X = 381;

                if (judgeframeint > 30) JudgeFrame.X = 110;
                if (judgeframeint < 30) JudgeFrame.X = 220;

                #region buttonBashing
                #region cheating
                if (keys.IsKeyDown(Keys.C)) cheating = true;
                else cheating = false;
                cheatRec.X = mouse.X;
                cheatRec.Y = mouse.Y;
                if (cheatRec.Intersects(cursorRec) && mouse.LeftButton == ButtonState.Pressed && cheating == true)
                {
                    cursorRec.X = mouse.X;
                }
                #endregion

                buttonsteptimer--;
                if (buttonsteptimer <= 0)
                {
                    buttontobebashed = rand.Next(1, 4);
                    buttonsteptimer = 120;
                }
                if (buttontobebashed == 1)
                {
                    KeytobeDisplayed = "A";
                    if (keys.IsKeyDown(Keys.A) && oldkey.IsKeyUp(Keys.A)) cursorRec.X += 14;
                    else cursorRec.X--;
                }
                if (buttontobebashed == 2)
                {
                    KeytobeDisplayed = "S";
                    if (keys.IsKeyDown(Keys.S) && oldkey.IsKeyUp(Keys.S)) cursorRec.X += 14;
                    else cursorRec.X--;
                }
                if (buttontobebashed == 3)
                {
                    KeytobeDisplayed = "D";
                    if (keys.IsKeyDown(Keys.D) && oldkey.IsKeyUp(Keys.D)) cursorRec.X += 15;
                    else cursorRec.X--;
                }

                oldkey = keys;
                #endregion
                #region expol
                exploTimer--;
                if (exploTimer <= 0)
                {
                    exploTimer = 10;
                }
                #endregion

                if (cursorRec.X < PortWidth / 50) { numofConvicted++; resetstage(1); }
                if (cursorRec.X > PortWidth / 1.1f) { numSaved++; resetstage(2); }

            }
            #endregion
            #region convicted
            if (result == 1)
            {
                if (accussedRec.X > (int)(0 - PortWidth / 10))
                {
                    accussedRec.X--;
                    if (accussedframeint > 0) accussedframeint--;
                    else accussedframeint = 30;

                    Accussedframe.Y = 207;
                    if (accussedframeint > 15) Accussedframe.X = 0;
                    if (accussedframeint < 15) Accussedframe.X = 45;
                }
                else
                {
                    result = 0;
                    start = false;
                }
            }
            #endregion
            #region Saved
            if (result == 2)
            {
                if (accussedRec.Y > (0 - accussedRec.Height))
                {
                    Accussedframe.Y = 103;
                    accussedRec.Y -= 3;
                    Accussedframe.X = 45;
                }
                else
                {
                    accussedRec.X = (int)(0 - PortWidth / 12);
                    accussedRec.Y = (int)(PortHeight / 2.1f);
                    result = 0;
                    start = false;
                }
            }
            #endregion
            //ends game
            if (start == false && numofConvicted + numSaved == 5)
            {
                state = GameState.End;
            }
        }
        void drawGame()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(BackgroundTex, BackgroundRec, Color.White);
            spriteBatch.Draw(hangBarTex, hangBarRec, Color.White);
            //people
            spriteBatch.Draw(JuryTex, JuryRec, JuryFrame, Color.White);
            spriteBatch.Draw(accussedTex, accussedRec, Accussedframe, Color.White);
            spriteBatch.Draw(judgeTex, judgeRec, JudgeFrame, Color.White);
            //bar
            spriteBatch.Draw(barTex, barRec, Color.White);
            spriteBatch.Draw(cursorTex, cursorRec, Color.White);
            if (cursorRec.X < PortWidth / 3) spriteBatch.Draw(cursorTex, cursorRec, new Color(255, 126, 0));
            if (cursorRec.X > PortWidth / 1.8f) spriteBatch.Draw(cursorTex, cursorRec, new Color(168, 230, 29));
            //button/text
            if (exploTimer > 5) spriteBatch.Draw(bashExploTex, cursorRec, Color.White);
            spriteBatch.DrawString(bashButton, "" + KeytobeDisplayed, new Vector2(cursorRec.X + (cursorRec.Width / 3f), cursorRec.Y + (cursorRec.Height / 18)), Color.Black);

            spriteBatch.DrawString(HelpFont, "Convicted: " + numofConvicted, new Vector2(PortWidth / 1.5f, PortHeight / 60), new Color(255, 126, 0));
            spriteBatch.DrawString(HelpFont, "Saved: " + numSaved, new Vector2(PortWidth / 1.5f, PortHeight / 20), new Color(168, 230, 29));

            if (cheating == true && arguing == true) spriteBatch.Draw(cheatTex, cheatRec, Color.White);
            spriteBatch.End();
        }

        void resetstage(int Result)
        {
            #region people

            Accussedframe.X = 46;
            JuryFrame.X = 127;
            JudgeFrame.X = 0;

            #endregion
            #region mechaincs

            cursorRec.X = (int)((PortWidth / 2) - ((PortWidth / 10)) / 2);
            KeytobeDisplayed = "";
            exploTimer = 0;
            buttonsteptimer = 0;
            buttontobebashed = 0;
            
            #endregion
            #region state

            arguing = false;
            result = Result;

            #endregion
        }

        #endregion

        #region End

        Rectangle ConLineRec;
        int resettimer;

        void loadEnd()
        {
            ConLineRec = new Rectangle((int)(PortWidth / 2), (int)(PortHeight / 4.3f), (int)(PortWidth / 50),(int)(PortHeight / 30));
        }
        void updateEnd()
        {
            if (numSaved == 5) state = GameState.Title;
            if (ConLineRec.X < (hangBarRec.Right - (PortWidth / 50)))
            {
                ConLineRec.X++;
                if (accussedframeint > 0) accussedframeint--;
                else accussedframeint = 30;

                Accussedframe.Y = 0;
                if (accussedframeint > 15) Accussedframe.X = 0;
                if (accussedframeint < 15) Accussedframe.X = 45;
                resettimer = 60;
            }
            else
            {
                resettimer--;
                Accussedframe.Y = 207;
                Accussedframe.X = 91;
            }
            if (resettimer == 0) state = GameState.Title;
        }
        void drawEnd()
        {
            spriteBatch.Begin();
            //convicted

            if (numofConvicted >= 1) spriteBatch.Draw(accussedTex, ConLineRec, Accussedframe, Color.White);

            if (numofConvicted >= 2) spriteBatch.Draw(accussedTex, new Rectangle((ConLineRec.X - ConLineRec.Width), ConLineRec.Y, ConLineRec.Width, ConLineRec.Height), Accussedframe, Color.White);

            if (numofConvicted >= 3) spriteBatch.Draw(accussedTex, new Rectangle((ConLineRec.X - (ConLineRec.Width * 2)), ConLineRec.Y, ConLineRec.Width, ConLineRec.Height), Accussedframe, Color.White);

            if (numofConvicted >= 4) spriteBatch.Draw(accussedTex, new Rectangle((ConLineRec.X - (ConLineRec.Width * 3)), ConLineRec.Y, ConLineRec.Width, ConLineRec.Height), Accussedframe, Color.White);

            if (numofConvicted >= 5) spriteBatch.Draw(accussedTex, new Rectangle((ConLineRec.X - (ConLineRec.Width * 4)), ConLineRec.Y, ConLineRec.Width, ConLineRec.Height), Accussedframe, Color.White);

            spriteBatch.Draw(BackgroundTex, BackgroundRec, Color.White);
            spriteBatch.Draw(hangBarTex, hangBarRec, Color.White);

            //people
            spriteBatch.Draw(JuryTex, JuryRec, JuryFrame, Color.White);
            spriteBatch.Draw(judgeTex, judgeRec, JudgeFrame, Color.White);

            spriteBatch.DrawString(HelpFont, "Convicted: " + numofConvicted, new Vector2(PortWidth / 1.5f, PortHeight / 60), new Color(255, 126, 0));
            spriteBatch.DrawString(HelpFont, "Saved: " + numSaved, new Vector2(PortWidth / 1.5f, PortHeight / 20), new Color(168, 230, 29));
            spriteBatch.End();
        }
        #endregion

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            PortHeight = GraphicsDevice.Viewport.Height;
            PortWidth = GraphicsDevice.Viewport.Width;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            loadTitle();
            loadGame();
            loadEnd();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            KeyboardState key = Keyboard.GetState();

            if (key.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here

            switch (state)
            {
                case GameState.Title:
                    updateTitle();
                    break;
                case GameState.Setup:
                    updateSetUp();
                    break;
                case GameState.Game:
                    updateGame();
                    break;
                case GameState.End:
                    updateEnd();
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            switch (state)
            {
                case GameState.Title:
                    drawTitle();
                    break;
                case GameState.Game:
                    drawGame();
                    break;
                case GameState.End:
                    drawEnd();
                    break;
            }

            base.Draw(gameTime);
        }
    }
}
