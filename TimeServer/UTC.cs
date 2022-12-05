using System;

namespace TimeClient
{
    public class UTC
    {
        private int utcZone;

        public int UTCZone
        {
            get { return utcZone; }
            set { utcZone = value; }
        }

        private string location;

        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        public UTC()
        {
            UTCZone = 0;
            Location = "Coordinated Universal Time";
        }

        public UTC(int utc, string location)
        {
            UTCZone = utc;
            Location = location;
        }

        public UTC(string utc, string location)
        {
            UTCZone = ConvertTimeToUTC(utc);
            Location = location;
        }

        public static int ConvertTimeToUTC(string input)
        {
            int result = 0;

            string formatInput;

            if (input.Length == 9)
            {
                formatInput = input.Substring(3);
            }
            else if (input.Length == 6)
            {
                formatInput = input;
            }
            else if (input.Length == 11)
            {
                formatInput = input.Substring(4, 6);
            }
            else
            {
                throw new Exception("Format UTC Wrong!");
            }

            string hour = formatInput.Substring(1, 2);
            string minus = formatInput.Substring(4);

            int valueHour = int.Parse(hour);
            int valueMunis = int.Parse(minus);

            result = valueHour * 60 + valueMunis;

            if (formatInput[0] == '-')
            {
                result = -result;
            }

            return result;
        }

        public static string ConvertValueUTCToString(int value)
        {
            string result = "";

            int tempValue = Math.Abs(value);

            int hour = tempValue / 60;

            int minus = Math.Abs(tempValue - hour * 60);

            if (hour < 10)
            {
                result += "0";
            }

            result += Math.Abs(hour).ToString() + ":";

            if (minus < 10)
            {
                result += "0";
            }

            result += minus.ToString();

            if (value < 0)
            {
                result = "-" + result;
            }

            if (result.Length != 6)
            {
                result = "+" + result;
            }

            return result;
        }

        public override string ToString()
        {
            return "(UTC" + ConvertValueUTCToString(utcZone) + ") " + location;
        }
    }
}
