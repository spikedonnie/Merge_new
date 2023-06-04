using System;
using System.IO;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public static class NumberToSymbol
{
    static readonly string[] symbol = new string[] { "","K", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J","L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",};
    
    public static string ChangeNumber(double number)
    {


        string zero = "0";
        
        if (-1d < number && number < 1d)
        {
            return zero;
        }

        if (double.IsInfinity(number))
        {
            return "Max";
        }

        //  부호 출력 문자열
        string significant = (number < 0) ? "-" : string.Empty;

        //  보여줄 숫자
        string showNumber = string.Empty;

        //  단위 문자열
        string unityString = string.Empty;

        //  패턴을 단순화 시키기 위해 무조건 지수 표현식으로 변경한 후 처리
        string[] partsSplit = number.ToString("E").Split('+');

        //  예외
        if (partsSplit.Length < 2)
        {
            return zero;
        }

        //  지수 (자릿수 표현)
        if (!int.TryParse(partsSplit[1], out int exponent))
        {
            Debug.LogWarningFormat("Failed - ToCurrentString({0}) : partSplit[1] = {1}", number, partsSplit[1]);
            return zero;
        }

        //  몫은 문자열 인덱스
        int quotient = exponent / 3;

        //  나머지는 정수부 자릿수 계산에 사용(10의 거듭제곱을 사용)
        int remainder = exponent % 3;

        //  1A 미만은 그냥 표현
        if (exponent < 3)
        {
            showNumber = System.Math.Truncate(number).ToString();
        }
        else
        {
            //  10의 거듭제곱을 구해서 자릿수 표현값을 만들어 준다.
            var temp = double.Parse(partsSplit[0].Replace("E", "")) * System.Math.Pow(10, remainder);

            //  소수 둘째자리까지만 출력한다.
            //showNumber = temp.ToString("F").Replace(".0", "");
            showNumber = temp.ToString("F");
        }

        unityString = symbol[quotient];

        return string.Format("{0}{1}{2}", significant, showNumber, unityString);
    }



    //public static string ChangeNumber(float value)
    //{
    //    //char[] temp = new char[64];
    //    //string strNum = new string(temp);
    //    string strNum = string.Format("{0:F0}", value);
    //    //Debug.Log("넘겨받은 값:" + strNum);

    //    int strLength = strNum.Length;

    //    if (strLength < 4)
    //    {
    //        return strNum;
    //    }

    //    int unit = 7;

    //    string[] symbol = { "K", "A", "B", "C", "D", "E", "F" };

    //    for (int i = 0; i < symbol.Length; i++)
    //    {
    //        int b;
    //        unit += i * 3;
    //        //Debug.Log("골드값 : " + strNum + "길이 : " +  strLength);

    //        if (i == 0)//1~6자리숫자 1~10만 사이의 수는 다 이곳으로 들어옴
    //        {
    //            if (strLength < unit)
    //            {
    //                if (strLength == unit - (unit - 4))//4자리 숫자라면 앞에 두개만
    //                {
    //                    b = 3;
    //                    strNum = strNum.Substring(0, b);//앞에 3개 자르고
    //                    strNum = string.Format("{0}.{1}", strNum.Substring(0, strNum.Length - 2), strNum.Substring(strNum.Length - 2));
    //                }
    //                else
    //                {
    //                    b = ((i + 1) * 2);
    //                    var length = strNum.Length - b;
    //                    strNum = strNum.Substring(0, length);
    //                    strNum = string.Format("{0}.{1}", strNum.Substring(0, length - 1), strNum.Substring(length - 1));
    //                }

    //                strNum += symbol[i];
    //                break;
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("보유골드 렝스:"+strNum.Length);
    //            Debug.Log("Unit:"+unit);

    //            if (strLength < unit)
    //            {
    //                if (strLength == unit - 3)//9백만 9,000000  =  9.00 뒤에 2자리
    //                {
    //                    b = 3;
    //                    strNum = strNum.Substring(0, b);
    //                    strNum = string.Format("{0}.{1}", strNum.Substring(0, strNum.Length - 2), strNum.Substring(strNum.Length - 2));
    //                }
    //                else if (strLength == unit - 2)//9천만 90,000,000 = 99.0 앞에 두자리 뒤에 한자리
    //                {
    //                    b = 3;
    //                    strNum = strNum.Substring(0, b);
    //                    strNum = string.Format("{0}.{1}", strNum.Substring(0, strNum.Length - 1), strNum.Substring(strNum.Length - 1));
    //                }
    //                else//9억 900,000,000  = 900,0앞에 3자리 뒤에 한자리
    //                {
    //                    b = 4;
    //                    strNum = strNum.Substring(0, b);
    //                    strNum = string.Format("{0}.{1}", strNum.Substring(0, strNum.Length - 1), strNum.Substring(strNum.Length - 1));
    //                }
    //                strNum += symbol[i];
    //                break;
    //            }

    //        }
    //    }

    //    return strNum;
    //}


}
