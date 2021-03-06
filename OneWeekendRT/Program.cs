﻿using System;
using System.Drawing;
using System.Collections.Generic;


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

        public static Vec3 random()
        {
            var rand = new System.Random();
            return new Vec3(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());
        }

        public static Vec3 random(double min, double max)
        {
            var L = max - min;
            var rand = new System.Random();
            var x = rand.NextDouble() * L + min;
            var y = rand.NextDouble() * L + min;
            var z = rand.NextDouble() * L + min;
            return new Vec3(x, y, z);
        }
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
        public bool isFrontFace;
        public IMaterial material;

        public void determine_face_normal(Ray ray, Vec3 outward_normal)
        {
            isFrontFace = Vec3.dot(ray.Direction, outward_normal) < 0;
            normal = isFrontFace ? outward_normal : -outward_normal;
        }
    }

    interface IHittable
    {
        bool Hit(Ray r, double t_min, double t_max, out HitRecord rec);
    }

    class Sphere : IHittable
    {
        public Sphere(Point3 c, double r, IMaterial mat)
        {
            center = c;
            radius = r;
            material = mat;
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
                    rec.isFrontFace = false;
                    rec.material = material;
                    var outward_normal = (rec.p - center) / radius;
                    rec.determine_face_normal(r, outward_normal);
                    return true;
                }
                temp = (-half_b + root) / a;
                if (temp < t_max && temp > t_min)
                {
                    rec.t = temp;
                    rec.p = r.At(rec.t);
                    rec.normal = (rec.p - center) / radius;
                    rec.isFrontFace = false;
                    rec.material = material;
                    var outward_normal = (rec.p - center) / radius;
                    rec.determine_face_normal(r, outward_normal);
                    return true;
                }
            }


            rec.normal = Vec3.Zero;
            rec.p = Vec3.Zero;
            rec.t = 0.0;
            rec.isFrontFace = false;
            rec.material = material;
            return false;
        }

        public Point3 center;
        public double radius;
        IMaterial material;
    }

    class HittableList : IHittable
    {
        public List<IHittable> objects = new List<IHittable>();

        public HittableList() { }

        public HittableList(IHittable obj)
        {
            Add(obj);
        }

        public void Clear()
        {
            objects.Clear();
        }

        public void Add(IHittable obj)
        {
            objects.Add(obj);
        }

        public bool Hit(Ray r, double t_min, double t_max, out HitRecord rec)
        {
            HitRecord tempHit = new HitRecord();
            rec = tempHit;
            bool hitAnything = false;
            double closestSoFar = t_max;

            foreach(var obj in objects)
            {
                if(obj.Hit(r, t_min, closestSoFar, out tempHit))
                {
                    hitAnything = true;
                    closestSoFar = tempHit.t;
                    rec = tempHit;
                }
            }

            return hitAnything;
        }
    }

    interface IMaterial
    {
        bool Scatter(Ray rayIn, ref HitRecord rec, out color attenuation, out Ray scattered);
    }

    class Lambertian : IMaterial
    {
        public color albedo;

        public Lambertian(color a)
        {
            albedo = a;
        }

        public bool Scatter(Ray rayIn, ref HitRecord rec, out color attenuation, out Ray scattered)
        {
            var scatter_direction = rec.normal + RayTracer.RandomInUnitSphere();
            scattered = new Ray(rec.p, scatter_direction);
            attenuation = albedo;
            return true;
        }
    }

    class Metal : IMaterial
    {
        public Metal(color a, double r)
        {
            albedo = a;
            roughness = r < 1 ? r : 1;
        }

        public color albedo;
        public double roughness;

        public bool Scatter(Ray rayIn, ref HitRecord rec, out color attenuation, out Ray scattered)
        {
            Vec3 reflect = RayTracer.Reflect(rayIn.Direction.unit_vector(), rec.normal);
            scattered = new Ray(rec.p, reflect + roughness * RayTracer.RandomInUnitSphere());
            attenuation = albedo;
            return Vec3.dot(scattered.Direction, rec.normal) > 0;
        }
    }

    class Dielectric : IMaterial
    {
        public double ref_idx;

        public Dielectric(double ri)
        {
            ref_idx = ri;
        }

        public bool Scatter(Ray rayIn, ref HitRecord rec, out color attenuation, out Ray scattered)
        {
            attenuation = new color(1.0, 1.0, 1.0);
            double etai_over_etat = rec.isFrontFace ? 1.0 / ref_idx : ref_idx;

            var unit_direction = rayIn.Direction.unit_vector();
            var cos_theta = Math.Min(Vec3.dot(-unit_direction, rec.normal), 1.0);
            var sin_theta = Math.Sqrt(1 - cos_theta * cos_theta);
            if(etai_over_etat * sin_theta > 1)
            {
                var reflected = RayTracer.Reflect(unit_direction, rec.normal);
                scattered = new Ray(rec.p, reflected);
                return true;
            }

            var reflect_prob = schlick(cos_theta, etai_over_etat);
            if (RayTracer.RandomDouble() < reflect_prob)
            {
                var reflected = RayTracer.Reflect(unit_direction, rec.normal);
                scattered = new Ray(rec.p, reflected);
                return true;
            }

            var refracted = RayTracer.Refract(unit_direction, rec.normal, etai_over_etat);
            scattered = new Ray(rec.p, refracted);
            return true;
        }

        double schlick(double cosine, double ref_idx)
        {
            var r0 = (1 - ref_idx) / (1 + ref_idx);
            r0 = r0 * r0;
            return r0 + (1 - r0) * Math.Pow(1 - cosine, 5);
        }
    }


    class Camera
    {

        public Camera(int w, int h, double focalL)
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

        public Ray GetRay(double u, double v)
        {
            return new Ray(Origin, LowerLeftCorner + u * HorizontalStepVec + v * VerticalStepVec - Origin);
        }
    }


    class RayTracer
    {
        public RayTracer(int w, int h, double focalL)
        {
            camera = new Camera(w, h, focalL);
        }

        Camera camera;
        public Camera Camera
        {
            get { return camera; }
        }

        public static double RandomDouble()
        {
            Random rand = new Random();
            return rand.NextDouble();
        }

        public int SamplePerPixel
        {
            get { return 100; }
        }

        public color ComputeRayColor(Ray ray, IHittable world, int recursiveDepth)
        {
            if (recursiveDepth <= 0)
                return color.Zero;

            var hitRec = new HitRecord();
            // use t_min=0.001 to fix the shadow acne
            if(world.Hit(ray, 0.001, float.PositiveInfinity, out hitRec))
            {
                Ray scattered = new Ray();
                color attenuation = color.Zero;
                if(hitRec.material.Scatter(ray, ref hitRec, out attenuation, out scattered))
                {
                    return attenuation * ComputeRayColor(scattered, world, recursiveDepth - 1);
                }
                return color.Zero;
            }

            var unit_dir = ray.Direction.unit_vector();
            var t = 0.5 * (unit_dir.Y() + 1.0);
            color end = new color(1.0, 1.0, 1.0);
            color start = new color(0.5, 0.7, 1.0);
            return (1.0 - t) * end + t * start;
        }

        public void WriteOutputColorAt(int x, int y, color pixelColor, Bitmap bmp)
        {
            var r = pixelColor.R();
            var g = pixelColor.G();
            var b = pixelColor.B();
            // Divide the color total by the number of samples and gamma-correct for gamma=2.0.
            var scale = 1.0 / SamplePerPixel;

            r = Math.Sqrt(scale * r);
            g = Math.Sqrt(scale * g);
            b = Math.Sqrt(scale * b);

            color newColor = new color(r, g, b);

            bmp.SetPixel(x, Camera.OutputImageHeight - y - 1, newColor.ToCLRColor());
        }

        public static Vec3 RandomInUnitSphere()
        {
            Random rand = new Random();
            var a = rand.NextDouble() * Math.PI * 2;
            var z = (rand.NextDouble() - 0.5) * 2;
            var r = Math.Sqrt(1 - z * z);
            return new Vec3(r * Math.Cos(a), r * Math.Sin(a), z);
        }

        public static Vec3 RandomInHemiSphere(Vec3 normal)
        {
            var in_unit_sphere = RandomInUnitSphere();
            if (Vec3.dot(in_unit_sphere, normal) > 0)
                return in_unit_sphere;
            else
                return -in_unit_sphere;
        }

        public static Vec3 Reflect(Vec3 v, Vec3 n)
        {
            return v - 2 * Vec3.dot(v, n) * n;
        }

        public static Vec3 Refract(Vec3 uv, Vec3 n, double etai_over_etat)
        {
            var cos_theta = Vec3.dot(-uv, n);
            var r_out_parallel = etai_over_etat * (uv + cos_theta * n);
            var r_out_perp = -(Math.Sqrt(1 - r_out_parallel.length_squared()) * n);
            return r_out_parallel + r_out_perp;
        }

        public static int MaxRecursiveDepth
        {
            get { return 50; }
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            RayTracer rt = new RayTracer(800, 600, 1.0);
            var bmp = new Bitmap(rt.Camera.OutputImageWidth, rt.Camera.OutputImageHeight);
            HittableList world = new HittableList();
            world.Add(new Sphere(new Point3(0, 0, -1), 0.5, new Lambertian(new color(0.1, 0.2, 0.5))));
            world.Add(new Sphere(new Point3(0, -100.5, -1), 100, new Lambertian(new color(0.8, 0.8, 0))));
            world.Add(new Sphere(new Point3(1, 0, -1), 0.5, new Metal(new color(0.8, 0.6, 0.2), 0)));
            //world.Add(new Sphere(new Point3(-1, 0, -1), 0.5, new Metal(new color(0.8, 0.8, 0.8), 0.6)));
            world.Add(new Sphere(new Point3(-1, 0, -1), 0.5, new Dielectric(1.5)));

            for (int y = 0; y < rt.Camera.OutputImageHeight; ++y)
            {
                Console.WriteLine("Line: " + y.ToString() + "...");
                for (int x = 0; x < rt.Camera.OutputImageWidth; ++x)
                {
                    color pixelColor = color.Zero;
                    for (int s = 0; s < rt.SamplePerPixel; ++s)
                    {
                        var u = (double)(x + RayTracer.RandomDouble()) / (rt.Camera.OutputImageWidth - 1);
                        var v = (double)(y + RayTracer.RandomDouble()) / (rt.Camera.OutputImageHeight - 1);
                        Ray ray = rt.Camera.GetRay(u, v);
                        pixelColor += rt.ComputeRayColor(ray, world, RayTracer.MaxRecursiveDepth);
                    }
                    rt.WriteOutputColorAt(x, y, pixelColor, bmp);
                }
            }
            Console.WriteLine("Save out final image.");
            bmp.Save("/Users/jhq/Desktop/RT.bmp");
        }
    }
}
