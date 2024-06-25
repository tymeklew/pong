using Raylib_cs;
using System.Numerics;

namespace HelloWorld;

public class Ball(Vector2 position, Vector2 velocity, float radius = 25F)
{
    public readonly float Radius = radius;
    private Random random = new Random();
    private Vector2 _position = position;
    private Vector2 _velocity = velocity;

    public Vector2 GetPosition() => this._position;
    public Vector2 GetVelocity() => this._velocity;
    public void HorizontalFlip() => this._velocity.X *= -1;
    public void VerticalFlip() => this._velocity.Y *= -1;
    // Move the ball
    public void Update()
    {
        this._position += this._velocity;
        this._velocity *= 1.00005F;
    }

    public void CheckCollision(int width, int height)
    {
        if (this._position.Y + Radius >= height || this._position.Y - Radius <= 0)
            this.VerticalFlip();
    }

    public void Reset(int width, int height)
    {
        this._position.X = this.random.Next(width / 4, 3 * width / 4);
        this._position.Y = this.random.Next(height / 8, height - height / 8);
        this._velocity = new Vector2(15, 15);
    }

    public void CheckCollisionWithPad(Vector2 position, int width, int height)
    {
        // Above
        if (this._position.Y + Radius <= position.Y) return;
        else if (this._position.Y - Radius >= position.Y + height) return;

        if (this._position.X >= position.X)
        {
            // Right side
            if (this._position.X - position.X <= this.Radius)
            {
                this.HorizontalFlip();
            }
        }
        else
        {
            // Left side
            if (position.X - this._position.X <= this.Radius)
            {
                this.HorizontalFlip();
            }
        }
    }

    public bool HitLeftWall() => this._position.X - Radius <= 0;
    public bool HitRightWall(int width) => this._position.X + Radius >= width;
}

public class Pad(int x)
{
    public readonly int Height = 210;
    public readonly int Width = 25;
    private Vector2 _position = new Vector2(x, 100);

    public Vector2 GetPosition() => this._position;
    public void MoveDown(int height)
    {
        if (this._position.Y + Height < height)
            this._position.Y += 20;
    }
    public void MoveUp()
    {
        if (this._position.Y > 0)
            this._position.Y -= 20;
    }
}

class Program
{
    public static void Main()
    {
        bool playing = true;
        int width = Raylib.GetScreenWidth();
        int height = Raylib.GetScreenWidth();

        Raylib.InitWindow(width, height, "Pong");
        Raylib.ToggleFullscreen();
        Raylib.SetTargetFPS(60);


        width = Raylib.GetScreenWidth();
        height = Raylib.GetScreenHeight();

        Ball ball = new Ball(new Vector2(500, 500), new Vector2(15, 15));
        Pad leftPad = new Pad(120);
        Pad rightPad = new Pad(width - 120);

        int leftScore = 0;
        int rightScore = 0;



        while (true)
        {
            width = Raylib.GetScreenWidth();
            height = Raylib.GetScreenHeight();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);



            if (playing)
            {
                if (Raylib.IsKeyDown(KeyboardKey.Down))
                    rightPad.MoveDown(height);
                else if (Raylib.IsKeyDown(KeyboardKey.Up))
                    rightPad.MoveUp();

                if (Raylib.IsKeyDown(KeyboardKey.S))
                    leftPad.MoveDown(height);
                else if (Raylib.IsKeyDown(KeyboardKey.W))
                    leftPad.MoveUp();


                // Check if ball bounced on either side 
                if (ball.HitLeftWall())
                {
                    rightScore++;
                    ball.Reset(width, height);
                    playing = false;
                }
                else if (ball.HitRightWall(width))
                {
                    leftScore++;
                    ball.Reset(width, height);
                    playing = false;
                }


                ball.Update();
                ball.CheckCollision(width, height);

                ball.CheckCollisionWithPad(leftPad.GetPosition(), leftPad.Width, leftPad.Height);
                ball.CheckCollisionWithPad(rightPad.GetPosition(), rightPad.Width, rightPad.Height);
            }
            else
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Space))
                {
                    playing = true;
                }
            }

            Raylib.DrawCircleV(ball.GetPosition(), ball.Radius, Color.White);

            Raylib.DrawRectangleV(leftPad.GetPosition(), new Vector2(leftPad.Width, leftPad.Height), Color.White);
            Raylib.DrawRectangleV(rightPad.GetPosition(), new Vector2(rightPad.Width, rightPad.Height), Color.White);


            // Draw scores
            Raylib.DrawText(leftScore.ToString(), (width / 2) / 2, 20, 60, Color.White);
            Raylib.DrawText(rightScore.ToString(), (width / 2) + (width / 4), 20, 60, Color.White);

            // Code to draw dotted line in the middle 
            int y = 0;
            int x = width / 2;
            for (int i = 0; i < height / 110 + 1; i++)
            {
                Raylib.DrawRectangleV(new Vector2(x, y), new Vector2(10, 90), Color.White);
                y += 110;
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
