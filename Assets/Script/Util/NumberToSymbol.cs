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

        //  ��ȣ ��� ���ڿ�
        string significant = (number < 0) ? "-" : string.Empty;

        //  ������ ����
        string showNumber = string.Empty;

        //  ���� ���ڿ�
        string unityString = string.Empty;

        //  ������ �ܼ�ȭ ��Ű�� ���� ������ ���� ǥ�������� ������ �� ó��
        string[] partsSplit = number.ToString("E").Split('+');

        //  ����
        if (partsSplit.Length < 2)
        {
            return zero;
        }

        //  ���� (�ڸ��� ǥ��)
        if (!int.TryParse(partsSplit[1], out int exponent))
        {
            Debug.LogWarningFormat("Failed - ToCurrentString({0}) : partSplit[1] = {1}", number, partsSplit[1]);
            return zero;
        }

        //  ���� ���ڿ� �ε���
        int quotient = exponent / 3;

        //  �������� ������ �ڸ��� ��꿡 ���(10�� �ŵ������� ���)
        int remainder = exponent % 3;

        //  1A �̸��� �׳� ǥ��
        if (exponent < 3)
        {
            showNumber = System.Math.Truncate(number).ToString();
        }
        else
        {
            //  10�� �ŵ������� ���ؼ� �ڸ��� ǥ������ ����� �ش�.
            var temp = double.Parse(partsSplit[0].Replace("E", "")) * System.Math.Pow(10, remainder);

            //  �Ҽ� ��°�ڸ������� ����Ѵ�.
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
    //    //Debug.Log("�Ѱܹ��� ��:" + strNum);

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
    //        //Debug.Log("��尪 : " + strNum + "���� : " +  strLength);

    //        if (i == 0)//1~6�ڸ����� 1~10�� ������ ���� �� �̰����� ����
    //        {
    //            if (strLength < unit)
    //            {
    //                if (strLength == unit - (unit - 4))//4�ڸ� ���ڶ�� �տ� �ΰ���
    //                {
    //                    b = 3;
    //                    strNum = strNum.Substring(0, b);//�տ� 3�� �ڸ���
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
    //            Debug.Log("������� ����:"+strNum.Length);
    //            Debug.Log("Unit:"+unit);

    //            if (strLength < unit)
    //            {
    //                if (strLength == unit - 3)//9�鸸 9,000000  =  9.00 �ڿ� 2�ڸ�
    //                {
    //                    b = 3;
    //                    strNum = strNum.Substring(0, b);
    //                    strNum = string.Format("{0}.{1}", strNum.Substring(0, strNum.Length - 2), strNum.Substring(strNum.Length - 2));
    //                }
    //                else if (strLength == unit - 2)//9õ�� 90,000,000 = 99.0 �տ� ���ڸ� �ڿ� ���ڸ�
    //                {
    //                    b = 3;
    //                    strNum = strNum.Substring(0, b);
    //                    strNum = string.Format("{0}.{1}", strNum.Substring(0, strNum.Length - 1), strNum.Substring(strNum.Length - 1));
    //                }
    //                else//9�� 900,000,000  = 900,0�տ� 3�ڸ� �ڿ� ���ڸ�
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
