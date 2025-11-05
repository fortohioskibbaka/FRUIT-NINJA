using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace FRUIT_NINJA
{
    public class Game1 : Game
    {
        bool end = false;
        bool win = false;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Rectangle window;
        Random genrator;
        List<Texture2D> textures;
        Texture2D fruitback;
        List <Texture2D>fruit; 
        List <Rectangle> fruiterec;
        List<Texture2D> fruittextures;
        float seconds;
        float respawntime;
       Texture2D losescreen;
        private Texture2D swordtexture
            ;
        Vector2 swordposition;
        Texture2D winscreen;

        List<Vector2> fruitspeeds;
        MouseState MouseState, previousmousestate;
        SoundEffect swordsound;
        int score = 0;
        SpriteFont font;
        private double timer = 25;





























        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            window = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            genrator = new Random();
            
            fruit = new List <Texture2D> (); 
            fruiterec = new List<Rectangle>();
            fruittextures = new List<Texture2D>();
            fruitspeeds = new List<Vector2>();
            swordtexture = Content.Load<Texture2D>("fruit/sword");
            font = Content.Load<SpriteFont>("font/font");
            losescreen = Content.Load<Texture2D>("lose");
            winscreen = Content.Load<Texture2D>("win1");



            for (int i = 0; i < 8; i++)
            {
                int x = genrator.Next(0, window.Width - 25);
                int y = genrator.Next(0, window.Height - 25);

                fruiterec.Add(new Rectangle(x, y, 50, 50));
                fruitspeeds.Add(new Vector2 (genrator.Next (5), genrator.Next (5)));
            }
            seconds = 0f;
            respawntime = 1;



            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            fruitback = Content.Load<Texture2D>("fruit/fruitback");
            for (int i = 1; i <= 7; i++)
            {

                fruittextures.Add(Content.Load<Texture2D>($"fruit/fruit" + i));

            }
            for (int i = 0; i < fruiterec.Count; i++)
            {
                fruittextures.Add(fruittextures[genrator.Next(fruittextures.Count)]);

            }

            swordsound= Content.Load<SoundEffect>("sounds/swordsound");



        }




        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouseState = Mouse.GetState();
            swordposition = new Vector2(mouseState.X - swordtexture.Width / 100, mouseState.Y - swordtexture.Height / 100);
            previousmousestate = MouseState;
            MouseState = Mouse.GetState();
            seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (seconds >= respawntime)
            {


                int x = genrator.Next(0, window.Width - 50);
                int y = genrator.Next(0, window.Height - 50);
                fruiterec.Add(new Rectangle(x, y, 50, 50));
                fruittextures.Add(fruittextures[genrator.Next(fruittextures.Count)]);
                fruitspeeds.Add(new Vector2(genrator.Next(10), genrator.Next(10)));


                seconds = 0f;



            }
            if (MouseState.LeftButton == ButtonState.Pressed && previousmousestate.LeftButton != ButtonState.Pressed)
            {
                swordsound.Play();
                for (int i = 0; i < fruiterec.Count; i++)
                {
                    if (fruiterec[i].Contains(MouseState.Position))
                    {
                        fruiterec.RemoveAt(i);
                        fruittextures.RemoveAt(i);
                        fruitspeeds.RemoveAt(i);
                        i--;

                        score += 1;

                    }

                }
            }
            for (int i = 0; i < fruiterec.Count; i++)
            {
                Rectangle temp;
                temp = fruiterec[i];
                temp.X += (int)fruitspeeds[i].X;
                temp.Y += (int)fruitspeeds[i].Y;
                fruiterec[i] = temp;

                if (fruiterec[i].Top <= window.Top || fruiterec[i].Bottom >= window.Height)
                {
                    Vector2 tempspeed;
                    tempspeed = fruitspeeds[i];
                    tempspeed.Y *= -1;
                    fruitspeeds[i] = tempspeed;

                }
                if (fruiterec[i].Left <= window.Left || fruiterec[i].Right >= window.Right)
                {
                    Vector2 tempspeed;
                    tempspeed = fruitspeeds[i];
                    tempspeed.X *= -1;
                    fruitspeeds[i] = tempspeed;
                }




            }
            if (mouseState.LeftButton == ButtonState.Pressed && previousmousestate.LeftButton != ButtonState.Pressed)
            {
                Point mousepoint = new Point(mouseState.X, mouseState.Y);


            }
            if (score >= 20)
            {
                win = true;
            }
            timer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (timer <= 0)
                end = true;


            previousmousestate = mouseState;


            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _spriteBatch.Draw(fruitback, window, Color.White);

            _spriteBatch.Draw(swordtexture,swordposition - new Vector2 (swordtexture.Width/2, swordtexture.Height/2), Color.White);

            string scoreText = $"Score: {score}";
            string timerText = $"Time remaing: {Math.Max(0, Math.Ceiling(timer))}";

            _spriteBatch.DrawString(font, scoreText, new Vector2(10, 40), Color.White);
            _spriteBatch.DrawString(font, timerText, new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(font, " slash 20 fruit in 25 seconds to win", new Vector2 (200, 10), Color.White);


            for (int i = 0; i < fruiterec.Count; i++)
            {
                _spriteBatch.Draw(fruittextures[i], fruiterec[i], Color.White);
            }

            if (end)
            {
                _spriteBatch.Draw(losescreen, window, Color.White);
                Console.ReadLine
                ();
                
            }
            if (win)
            {

                _spriteBatch.Draw(winscreen, window, Color.White);
                Console.ReadLine();
                seconds = 0f;
            }





            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
