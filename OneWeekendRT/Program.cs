using System;
using System.Drawing;


namespace OneWeekendRT
{
    struct vec3
    {
        public vec3(float e1, float e2, float e3)
        {
            x = e1;
            y = e2;
            z = e3;
        }

        public float X() { return x; }
        public float Y() { return y; }
        public float Z() { return z; }
        public float R() { return x; }
        public float G() { return y; }
        public float B() { return z; }

        public static vec3 operator+(vec3 v) { return v; }
        public static vec3 operator-(vec3 v) { return new vec3(-v.x, -v.y, -v.z); }

        public float this[int index]
        {
            get
            {
                System.Diagnostics.Debug.Assert(index >= 0 && index <= 2);
                if (index == 0)
                    return x;
                else if (index == 1)
                    return y;
                else
                    return z;
            }
            set
            {
                System.Diagnostics.Debug.Assert(index >= 0 && index <= 2);
                if (index == 0)
                    x = value;
                else if (index == 1)
                    y = value;
                else
                    z = value;
            }
        }

        public static vec3 operator+(vec3 lhs, vec3 rhs)
        {
            return new vec3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }
        public static vec3 operator-(vec3 lhs, vec3 rhs)
        {
            return new vec3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }
        public static vec3 operator*(vec3 lhs, vec3 rhs)
        {
            return new vec3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        }
        public static vec3 operator/(vec3 lhs, vec3 rhs)
        {
            return new vec3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
        }
        public static vec3 operator*(vec3 lhs, float t)
        {
            return new vec3(lhs.x * t, lhs.y * t, lhs.z * t);
        }
        public static vec3 operator*(float t, vec3 rhs)
        {
            return new vec3(t * rhs.x, t * rhs.y, t * rhs.z);
        }
        public static vec3 operator/(vec3 lhs, float t)
        {
            return new vec3(lhs.x / t, lhs.y / t, lhs.z / t);
        }

        public float length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public float square_length()
        {
            return x * x + y * y + z * z;
        }

        public vec3 unit_vector()
        {
            return this / length();
        }

        public static float dot(vec3 lhs, vec3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        public static vec3 cross(vec3 lhs, vec3 rhs)
        {
            return new vec3(lhs.y * rhs.z - lhs.z * rhs.y,
                -(lhs.x * rhs.z - lhs.z * rhs.x),
                lhs.x * rhs.y - lhs.y * rhs.x
                );
        }


        float x, y, z;
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var bmp = new Bitmap(300, 300);
            for(int x = 0; x < 300; ++x)
            {
                for(int y = 0; y < 300; ++y)
                {
                    bmp.SetPixel(x, y, Color.Red);
                }
            }
            bmp.Save("/Users/jhq/Desktop/RT.bmp");
        }
    }
}
