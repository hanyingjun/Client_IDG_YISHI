﻿using UnityEngine;

namespace IDG
{
    /// <summary>
    /// 定点数二维向量
    /// </summary>
    [System.Serializable]
    public struct Fixed2
    {
        public Fixed x;
        public Fixed y;

        public static bool DistanceLess(Fixed2 a,Fixed2 b,Fixed len){
            var xLen=a.x-b.x;
            var yLen=a.y-b.y;
            return (xLen*xLen+yLen*yLen)<len*len;
        }
        public static Fixed2 one=new Fixed2(1,1);
        public Fixed2(float x, float y)
        {

            this.x = new Fixed(x);
            this.y = new Fixed(y);

        }
        public Fixed2(Fixed x, Fixed y)
        {
            this.x = x;
            this.y = y;

        }
        public Vector3 ToVector3(int yValue=0)
        {
            return new Vector3(x.ToFloat(),yValue, y.ToFloat());
        }
        public static Fixed2 GetV2(Fixed x, Fixed y)
        {
            return new Fixed2(x, y);
        }
        public static Fixed2 operator +(Fixed2 a, Fixed2 b)
        {
            return new Fixed2(a.x + b.x, a.y + b.y);
        }
        public static Fixed2 operator -(Fixed2 a, Fixed2 b)
        {
            return new Fixed2(a.x - b.x, a.y - b.y);
        }
        public static Fixed2 operator *(Fixed2 a, Fixed b)
        {
            return new Fixed2(a.x * b, a.y * b);
        }
        public Fixed2 Rotate(Fixed value)
        {
            Fixed tx, ty;
            tx = MathFixed.CosAngle(value) * x - y * MathFixed.SinAngle(value);
            ty = MathFixed.CosAngle(value) * y + x * MathFixed.SinAngle(value);
            return new Fixed2(tx, ty);
        }
        
        public Fixed ToRotation()
        {
            if (x == 0 && y == 0)
            {
                return new Fixed();
            }
            Fixed sin = this.normalized.y;
            Fixed result=Fixed.Zero;
            if (this.x >= 0)
            {
                result= MathFixed.Asin(sin)/MathFixed.PI*180;
            }
            else
            {
                result= MathFixed.Asin(-sin) / MathFixed.PI * 180+180;
            }
            // if(result==0){
            //     Debug.LogError("this.normalized "+this.normalized+" MathFixed.Asin(sin) "+MathFixed.Asin(sin));
            // }
            return result;
        }
        public static Fixed2 Parse(Fixed ratio)
        {
            return new Fixed2(MathFixed.CosAngle(ratio), MathFixed.SinAngle(ratio) );
        }
        public Fixed2 normalized
        {

            get
            {
                if (x == 0 && y == 0)
                {
                    return new Fixed2();
                }
                Fixed n = ((x * x) + (y * y)).Sqrt();
                //   Debug.Log("N" + ((x * x) + (y * y)).Sqrt());
                var result=new Fixed2(x / n, y / n);
                result.x= Fixed.Range(result.x,-1,1);
                result.y= Fixed.Range(result.y,-1,1);
                return result;

            }
        }
        public Fixed magnitude
        {

            get {
                if (x == 0 && y == 0)
                {
                    return Fixed.Zero;
                }
                Fixed n =((x *x) + (y * y)).Sqrt();
             //   Debug.Log("N" + ((x * x) + (y * y)).Sqrt());
                return n;

            }
        }
        //public static V2 operator *(Ratio a, V2 b)
        //{
        //    return new V2(a*b.x,  a* b.y);
        //}
        public static Fixed2 left = new Fixed2(-1, 0);
        public static Fixed2 right = new Fixed2(1, 0);
        public static Fixed2 up = new Fixed2(0, 1);
        public static Fixed2 down = new Fixed2(0, -1);
        public static Fixed2 zero = new Fixed2(0, 0);
        //public static V3 operator +(V3 v3,Ratio ratio)
        //{

        //}
        public Fixed Dot(Fixed2 b)
        {
            return Dot(this, b);
        }
        public static Fixed Dot(Fixed2 a,Fixed2 b)
        {
            return a.x*b.x+b.y*a.y;
        }

        public static Fixed2 operator -(Fixed2 a)
        {
            return new Fixed2(-a.x, -a.y);
        }
        public static Fixed3 operator *(Fixed2 a, Fixed2 b)
        {
            return new Fixed3(new Fixed(),new Fixed(),  a.x * b.y - a.y * b.x);
        }
        public static bool operator ==(Fixed2 a, Fixed2 b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Fixed2 a, Fixed2 b)
        {
            return a.x != b.x || a.y != b.y;
        }
        public override string ToString()
        {
            return "{" + x.ToString() + "," + y.ToString() + "}";// + ":" + ToVector3().ToString();
        }

    }
}
