using System.Data;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Calculation
{
    
    public class Calculator : MonoBehaviour
    {

        #region Main Methods
        
        public float Calculate(int[] numbers, MathItem.Sign[] signs)
        {
            string expression = ParseExpression(numbers, signs);
            DataTable dt = new DataTable();
            var v = dt.Compute(expression,"");
            return float.Parse(v.ToString()); 
        }

        private string ParseExpression(int[] numbers, MathItem.Sign[] signs)
        {
            string result = "0";
            if (numbers[0] != 0)
            {
                result = ""+numbers[0];
                if (numbers[1] != 0 && signs[0] != MathItem.Sign.None)
                {
                    result = result + GetSignString(signs[0]) + numbers[1];
                    if (numbers[2] != 0 && signs[1] != MathItem.Sign.None)
                    {
                        result += GetSignString(signs[1]) + numbers[2];
                        if (numbers[3] != 0 && signs[2] != MathItem.Sign.None)
                        {
                            result += GetSignString(signs[2]) + numbers[3];
                        }
                    }
                }
            }

            return result;
        }

        private string GetSignString(MathItem.Sign sign)
        {
            switch (sign)
            {
                case MathItem.Sign.Add:
                    return "+";
                case MathItem.Sign.Sub:
                    return "-";
                case MathItem.Sign.Mul:
                    return "*";
                case MathItem.Sign.Div:
                    return "/";
                default:
                    print("wrong sign");
                    return "";
            }
        }
        
        #endregion
    }
}
