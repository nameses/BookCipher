﻿using System;
using System.Collections.Generic;

class VerseCipher
{
    private char[,] keyGrid;
    private Dictionary<char, List<string>> charToCodeMap = new Dictionary<char, List<string>>();
    private Dictionary<string, char> codeToCharMap = new Dictionary<string, char>();

    public VerseCipher(string verse, int rows, int cols)
    {
        keyGrid = new char[rows, cols];

        FillKeyGrid(verse, rows, cols);
        BuildMappings();
    }

    private void FillKeyGrid(string verse, int rows, int cols)
    {
        int index = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (index < verse.Length)
                {
                    keyGrid[i, j] = verse[index];
                    index++;
                }
            }
        }
    }

    private void BuildMappings()
    {
        int rows = keyGrid.GetLength(0);
        int cols = keyGrid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                char ch = keyGrid[i, j];
                string code = $"{i + 1}/{j + 1}";

                if (!charToCodeMap.ContainsKey(ch))
                {
                    charToCodeMap[ch] = new List<string>();
                }
                charToCodeMap[ch].Add(code);
                codeToCharMap[code] = ch;
            }
        }
    }

    public string Encrypt(string message)
    {
        Random rand = new Random();
        List<string> cipherText = new List<string>();

        foreach (char ch in message)
        {
            if (charToCodeMap.ContainsKey(ch))
            {
                List<string> codes = charToCodeMap[ch];
                string selectedCode = codes[rand.Next(codes.Count)];
                cipherText.Add(selectedCode);
            }
            else
            {
                cipherText.Add("??/??");
            }
        }

        return string.Join(",", cipherText);
    }

    public string Decrypt(string cipherText)
    {
        string[] codes = cipherText.Split(',');
        List<char> message = new List<char>();

        foreach (string code in codes)
        {
            if (codeToCharMap.ContainsKey(code))
            {
                message.Add(codeToCharMap[code]);
            }
            else
            {
                message.Add('?');
            }
        }

        return new string(message.ToArray());
    }
}