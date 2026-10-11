using SettingsClone.ViewModels.Field;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SettingsClone.ViewModels.Field.RangeFieldViewModel;

namespace SettingsClone.Services.Field;

public class FieldsService : IMessageToken
{
    Random random = new();

    public StringBuilder Elaborate_field(MessageFieldViewModel field)
    {
        StringBuilder token_string = new();

        switch (field)
        {
            /*....................................................................
            @   
            @   case of Textfield type
            @....................................................................*/
            case TextFieldViewModel textToken:
                token_string.Append(textToken.Value);
                break;


            /*....................................................................
            @   
            @   case of Choice field type
            @....................................................................*/
            case ChoiceFieldViewModel choiceToken:
                try
                {
                    var randomOption = choiceToken.Options?[random.Next(choiceToken.Options.Count)];
                    token_string.Append(randomOption);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.WriteLine("ArgumentOutOfRangeException");
                }
                break;


            /*....................................................................
            @   
            @   case of Range field type
            @....................................................................*/
            case RangeFieldViewModel rangeToken:

                switch (rangeToken.Format)
                {
                    /*....................................................................
                    @   
                    @   case of Range.Numeric field subtype
                    @....................................................................*/
                    case RangeType.Numeric:
                        try
                        {
                            int length;

                            length = Math.Max(rangeToken.Start.Length, rangeToken.End.Length);

                            var randomOption = random.Next(int.Parse(rangeToken.Start), int.Parse(rangeToken.End) + 1);
                            token_string.Append(randomOption.ToString($"D{length}"));
                        }
                        catch (FormatException e)
                        {
                            Debug.WriteLine("FormatException");
                        }
                        catch (NullReferenceException e)
                        {
                            Debug.WriteLine("NullReferenceException");
                        }
                        break;
                    /*....................................................................
                    @   
                    @   case of Range.Alphanumeric field subtype
                    @....................................................................*/
                    case RangeType.AlphaNumeric:
                        {
                            StringBuilder randomOption = new();
                            int length;

                            length = Math.Max(rangeToken.Start.Length, rangeToken.End.Length);

                            for (int i = 0; i < length; i++)
                            {
                                char start = rangeToken.Start[i];
                                char end = rangeToken.End[i];

                                randomOption.Append(GenerateRandomChar(start, end));
                            }

                            token_string.Append(randomOption);

                        }
                        break;
                    /*....................................................................
                    @   
                    @   case of Range.Alphabetic field subtype
                    @....................................................................*/
                    case RangeType.Alphabetic:
                        try
                        {
                            StringBuilder randomOption = new();
                            int length;

                            length = Math.Max(rangeToken.Start.Length, rangeToken.End.Length);

                            //A = 65
                            //Z = 90
                            //a = 97
                            //z = 122
                            for (int i = 0; i < length; i++)
                            {
                                char start = rangeToken.Start[i];
                                char end = rangeToken.End[i];

                                int temp;
                                if (start > end)
                                {
                                    temp = start;
                                    start = end;
                                    end = (char)temp;
                                }

                                // between 65 - 90
                                if ((start >= 'A' && start <= 'Z') && (end >= 'A' && end <= 'Z'))
                                {
                                    randomOption.Append(
                                        (char)random.Next(start, end + 1));
                                }
                                // between 97 - 122
                                else if ((start >= 'a' && start <= 'z') && (end >= 'a' && end <= 'z'))
                                {
                                    randomOption.Append(
                                        (char)random.Next(start, end + 1));
                                }
                                else if ((start >= 'A' && start <= 'Z') &&
                                        (end >= 'a' && end <= 'z'))
                                {
                                    List<char> options = new();

                                    options.Add((char)random.Next('a', end + 1));
                                    options.Add((char)random.Next(start, 'Z' + 1));

                                    randomOption.Append(
                                        options[random.Next(options.Count)]);
                                }
                                else if ((start >= 'a' && start <= 'z') &&
                                        (end >= 'A' && end <= 'Z'))
                                {
                                    List<char> options = new();

                                    options.Add((char)random.Next(start, 'z' + 1));
                                    options.Add((char)random.Next('A', end + 1));

                                    randomOption.Append(
                                        options[random.Next(options.Count)]);
                                }
                                else
                                {
                                    randomOption.Append('.');
                                }
                            }

                            token_string.Append(randomOption);
                        }
                        catch (FormatException e)
                        {
                            Debug.WriteLine("test");
                        }
                        break;
                }

                break;


            /*....................................................................
            @   
            @   case of Counter field type
            @....................................................................*/
            case CounterFieldViewModel counterToken:
                try
                {
                    int length;

                    length = Math.Max(counterToken.Start.Length, counterToken.End.Length);


                    if (counterToken.Current > int.Parse(counterToken.End))
                    {
                        counterToken.Current = int.Parse(counterToken.Start);
                    }

                    token_string.Append((counterToken.Current++).ToString($"D{length}"));
                }
                catch (FormatException e)
                {
                    Debug.WriteLine("test");
                }



                break;
            _:
                break;

        }

        return token_string;
    }


    private char GenerateRandomChar(char start, char end)
    {

        if (IsDigit(start) && IsDigit(end))
        {
            return (char)random.Next(start, end + 1);
        }
        /*....................................................................
        @   
        @   case of 'A' and 'Z'
        @....................................................................*/
        if (IsUpper(start) && IsUpper(end))
        {
            if (start > end)
            {
                (start, end) = (end, start);
            }
            return (char)random.Next(start, end + 1);
        }
        /*....................................................................
        @   
        @   case of 'a' and 'z'
        @....................................................................*/
        if (IsLower(start) && IsLower(end))
        {
            if (start > end)
            {
                (start, end) = (end, start);
            }
            return (char)random.Next(start, end + 1);
        }
        /*....................................................................
        @   
        @   case of 'a' and 'Z'
        @....................................................................*/
        if (IsLower(start) && IsUpper(end))
        {
            List<char> options = new();

            options.Add((char)random.Next(start, 'z' + 1));
            options.Add((char)random.Next('A', end + 1));

            return (char)options[random.Next(options.Count)];
        }
        /*....................................................................
        @   
        @   case of 'A' and 'z'
        @....................................................................*/
        if (IsUpper(start) && IsLower(end))
        {

            List<char> options = new();

            options.Add((char)random.Next(start, 'Z' + 1));
            options.Add((char)random.Next('a', end + 1));

            return (char)options[random.Next(options.Count)];

        }
        return '.';
    }
    /*....................................................................
    @
    @
    @       Check if the char is UPPER case
    @
    @
    @....................................................................*/
    private static bool IsUpper(char c)
    {
        return c >= 'A' && c <= 'Z';
    }
    /*....................................................................
    @
    @
    @       Check if the char is lower case
    @
    @
    @....................................................................*/
    private static bool IsLower(char c)
    {
        return c >= 'a' && c <= 'z';
    }
    /*....................................................................
    @
    @
    @       Check if the char is a number digit
    @
    @
    @....................................................................*/
    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }
}
