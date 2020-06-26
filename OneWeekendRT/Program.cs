using System;
using System.Drawing;


namespace OneWeekendRT
{
    using Point3 = Vec3;
    using color = Vec3;

    struct Vec3
    {
        public Vec3(double e1, double e2, double e3)
        {
            x = e1;
            y = e2;
            z = e3;
        }

        public double X() { return x; }
        public double Y() { return y; }
        public double Z() { return z; }
        public double R() { return x; }
        public double G() { return y; }
        public double B() { return z; }

        public static Vec3 operator+(Vec3 v) { return v; }
        public static Vec3 operator-(Vec3 v) { return new Vec3(-v.x, -v.y, -v.z); }

        public double this[int index]
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

        public static Vec3 operator+(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }
        public static Vec3 operator-(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }
        public static Vec3 operator*(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        }
        public static Vec3 operator/(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
        }
        public static Vec3 operator*(Vec3 lhs, double t)
        {
            return new Vec3(lhs.x * t, lhs.y * t, lhs.z * t);
        }
        public static Vec3 operator*(double t, Vec3 rhs)
        {
            return new Vec3(t * rhs.x, t * rhs.y, t * rhs.z);
        }
        public static Vec3 operator/(Vec3 lhs, double t)
        {
            return new Vec3(lhs.x / t, lhs.y / t, lhs.z / t);
        }

        public double length()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public double length_squared()
        {
            return x * x + y * y + z * z;
        }

        public Vec3 unit_vector()
        {
            return this / length();
        }

        public static double dot(Vec3 lhs, Vec3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        public static Vec3 cross(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(lhs.y * rhs.z - lhs.z * rhs.y,
                -(lhs.x * rhs.z - lhs.z * rhs.x),
                lhs.x * rhs.y - lhs.y * rhs.x
                );
        }

        public Color ToCLRColor()
        {
            var ret = Color.FromArgb(
                (int)(x * 255),
                (int)(y * 255),
                (int)(z * 255)
                );
            return ret;
        }


        public double x, y, z;

        public static Vec3 Zero = new Vec3(0.0, 0.0, 0.0);
    }


    struct Ray
    {
        public Point3 origin;
        public Vec3 dir;

        public Ray(Point3 o, Vec3 d)
        {
            origin = o;
            dir = d;
        }

        public Point3 Origin
        {
            get
            {
                return origin;
            }
        }

        public Vec3 Direction
        {
            get
            {
                return dir;
            }
        }

        public Point3 At(double t)
        {
            return Origin + t * Direction;
        }
    }

    struct HitRecord
    {
        public Point3 p;
        public Vec3 normal;
        public double t;
    }

    interface IHittable
    {
        bool Hit(Ray r, double t_min, double t_max, out HitRecord rec);
    }

    class Sphere : IHittable
    {
        public Sphere(Point3 c, double r)
        {
            center = c;
            radius = r;
        }

        public bool Hit(Ray r, double t_min, double t_max, out HitRecord rec)
        {
            Vec3 oc = r.Origin - center;
            var a = r.Direction.length_squared();
            var half_b = Vec3.dot(oc, r.Direction);
            var c = oc.length_squared() - radius * radius;
            var discriminant = half_b * half_b - a * c;

            if (discriminant > 0)
            {
                var root = Math.Sqrt(discriminant);
                var temp = (-half_b - root) / a;
                if (temp < t_max && temp > t_min)
                {
                    rec.t = temp;
                    rec.p = r.At(rec.t);
                    rec.normal = (rec.p - center) / radius;
                    return true;
                }
                temp = (-half_b + root) / a;
                if (temp < t_max && temp > t_min)
                {
                    rec.t = temp;
                    rec.p = r.At(rec.t);
                    rec.normal = (rec.p - center) / radius;
                    return true;
                }
            }


            rec.normal = Vec3.Zero;
            rec.p = Vec3.Zero;
            rec.t = 0.0;
            return false;
        }

        public Point3 center;
        public double radius;
    }


    class RayTracer
    {
        public RayTracer(int w, int h, double focalL)
        {
            outputImageHeight = h;
            outputImageWidth = w;
            focalLength = focalL;
        }

        int outputImageWidth;
        int outputImageHeight;
        public int OutputImageWidth
        {
            get { return outputImageWidth; }
        }
        public int OutputImageHeight
        {
            get { return outputImageHeight; }
        }

        public double ViewportWidth
        {
            get { return ViewportHeight * AspectRatio; }
        }

        public double ViewportHeight
        {
            get { return 2.0f; }
        }

        double focalLength;
        public double FocalLength
        {
            get { return focalLength; }
        }

        public double AspectRatio
        {
            get
            {
                return (double)OutputImageWidth / (double)OutputImageHeight;
            }
        }

        public Point3 Origin
        {
            get { return Point3.Zero; }
        }

        public Vec3 HorizontalStepVec
        {
            get
            {
                return new Vec3(ViewportWidth, 0, 0);
            }
        }

        public Vec3 VerticalStepVec
        {
            get
            {
                return new Vec3(0, ViewportHeight, 0);
            }
        }

        public Point3 LowerLeftCorner
        {
            get
            {
                var depth = new Point3(0, 0, FocalLength);
                return Origin - HorizontalStepVec / 2 - VerticalStepVec / 2 - depth;
            }
        }

        public color ComputeRayColor(Ray ray)
        {
            var sphereCenter = new Point3(0, 0, -1);
            var sphere = new Sphere(sphereCenter, 0.5);
            var hitRec = new HitRecord();
            var hit = sphere.Hit(ray, 0.0, 1.0, out hitRec);
            if (hit)
            {
                var N = (ray.At(hitRec.t) - sphereCenter).unit_vector();
                return 0.5 * (new color(N.X() + 1, N.Y() + 1, N.Z() + 1));
            }

            var unit_dir = ray.Direction.unit_vector();
            var t = 0.5 * (unit_dir.Y() + 1.0);
            color end = new color(1.0, 1.0, 1.0);
            color start = new color(0.5, 0.7, 1.0);
            return (1.0 - t) * end + t * start;
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            RayTracer rt = new RayTracer(800, 600, 1.0);
            var bmp = new Bitmap(rt.OutputImageWidth, rt.OutputImageHeight);
            for (int y = 0; y < rt.OutputImageHeight; ++y)
            {
                for (int x = 0; x < rt.OutputImageWidth; ++x)
                {
                    var u = (double)x / (rt.OutputImageWidth - 1);
                    var v = (double)y / (rt.OutputImageHeight - 1);
                    Ray ray = new Ray(rt.Origin,
                        rt.LowerLeftCorner + u * rt.HorizontalStepVec + v * rt.VerticalStepVec - rt.Origin);
                    var pixelColor = rt.ComputeRayColor(ray);
                    bmp.SetPixel(x, rt.OutputImageHeight - y - 1, pixelColor.ToCLRColor());
                }
            }
            bmp.Save("/Users/jhq/Desktop/RT.bmp");
        }
    }
}
