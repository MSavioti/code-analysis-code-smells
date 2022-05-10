using System;
using System.Collections.Generic;

namespace TP3.Test
{
    public class DummyClass
    {
        private int _x;
        private int _y;

        private class InternalClass
        {
            private int _z;

            public int GetZ()
            {
                return _z;
            }

            public void SetZ(int value)
            {
                if (value >= 0 && value < 12)
                    _z = value;
            }
        }
        
        #region Method length analysing

        public bool LongMethod()
        {
            int limit = 101;
            List<int> dummyList;
            dummyList = new List<int>();

            // Não faz absolutamente nada.
            for (int i = 0; i < limit; i++)
            {
                for (int j = 0; j < limit; j++)
                {
                    for (int k = 0; k < limit; k++)
                    {
                        dummyList.Add(i + j + k);
                    }
                }
            }

            // Não faz nada.
            double x = 1;
            double y = 2;
            double z = System.Math.Pow(x, y);

            for (int i = 0; i < limit; i++)
            {
                for (int j = 0; j < limit; j++)
                {
                    dummyList[i] *= -j;
                }
            }

            if (dummyList[0] * z > limit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AverageSizeMethod()
        {
            int limit = 10;
            List<int> dummyList;
            dummyList = new List<int>();

            for (int i = 0; i < limit; i++)
            {
                for (int j = 0; j < limit; j++)
                {
                    for (int k = 0; k < limit; k++)
                    {
                        dummyList.Add(i + j + k);
                    }
                }
            }
        }

        public bool ShortMethod()
        {
            Random random = new Random();
            return random.Next(0, 100) < 50;
        }

        #endregion

        #region Parameter count analysing

        public void NoParameters()
        {
            return;
        }

        public int FewParameters(int x, int y)
        {
            return x + y;
        }

        public int ThreeParameters(int x, int y, int z)
        {
            return x + y + z;
        }

        public int FourParameters(int x, int y, int z, int w)
        {
            return x + y + z + w;
        }

        public int LotOfParameters(int x, int y, int z, int w, int v, int u)
        {
            return x + y + z + w + v + u;
        }

        #endregion

        #region Magic attributes

        public string GetFixedString(string value)
        {
            return "Magic " + value;
        }

        public float GetACoolNumber()
        {
            float f = 1.5f;

            for (int i = 0; i < 15; i++)
            {
                f += 7.5f;
            }
            
            return f;
        }

        public string NumberInString()
        {
            string s = "test: ";
            return s.Replace('s', 'X');
        }

        #endregion

        #region Getters and setters

        public int GetX()
        {
            return _x;
        }

        public int GetY() { return _y; }

        public void SetX(int value)
        {
            _x = value;
        }

        public void SetY(int value) { _y = value; }

        #endregion


    }
}
