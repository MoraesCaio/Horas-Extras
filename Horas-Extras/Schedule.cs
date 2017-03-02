using System;
using System.Collections;
using System.Linq;

/*Class used to obtain the sum of extra hours.
 * Input: CSV file lines
 * It's mainly designed regarding manipulation of TimeSpan objects.
 * Author: Caio Moraes
 * GitHub: MoraesCaio
 * Email: caiomoraes@msn.com
 **/
namespace Horas_Extras
{
    class Schedule
    {
        //Official schedule's hours
        //Can be changed in the UI
        public static string[] official = new string[] { "8:00", "12:00", "13:00", "17:00" };
        public static TimeSpan[] officialTS = new TimeSpan[4] {
            new TimeSpan( 8, 0, 0),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            new TimeSpan(17, 0, 0)
        };
        //Format used on parse methods
        private static string format = "h\\:m";

        /*Retrieves the index of the nTh ocurrence of a specified character on the string.
         * Parameters:
         * string s, the string that will be looked on;
         * char c, the character that will be searched;
         * int n, the number of ocurrences to be ignored + 1;
         *
         * Returns:
         * int, index of the nTh ocurrence of the character;
         * Returns -1, in case it fails.
         */
        private static int nIndexOf(string s, char c, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        /*A validation method to set the official hours.
         * Parameters:
         *  string hour, string that will be parsed to a TimeSpan object in the format hh:mm
         *  int idx, index of the main four shifts:
         *      0 - arrival
         *      1 - lunch time
         *      2 - end of lunch
         *      4 - end of work
         * Returns:
         *  false - if it's not possible to parse the string
         *  true - otherwise
         **/
        public static bool SetOfficialHour(string hour, int idx){
            if(hour != official[idx]){
                if(!TimeSpan.TryParseExact(hour, format, null, out officialTS[idx])){
                    return false;
                }
                official[idx] = hour;
            }
            return true;
        }
        /*Adds up every extra hour and minute of work and subtracts in case the worker arrived late or got out earlier.
         * It takes into account official hours.
         * Parameters:
         *  string[] lines, each line must be in the format hh:mm
         * Returns:
         *  Timespan, total of extra days, hours and minutes
         */
        public static TimeSpan AddExtraHours(string[] lines){
            ArrayList dailyChecks = new ArrayList();
            foreach(string line in lines){
                dailyChecks.Add(Schedule.ParseLine(line));
            }
            TimeSpan total = TimeSpan.Zero;
            foreach(TimeSpan[] ts in dailyChecks)
            {
                total = total.Add(Schedule.officialTS[0].Subtract(ts[0]));
                total = total.Add(ts[1].Subtract(Schedule.officialTS[1]));
                total = total.Add(Schedule.officialTS[2].Subtract(ts[2]));
                total = total.Add(ts[3].Subtract(Schedule.officialTS[3]));
            }
            return total;
        }
        /*Try parse a line read from a csv file in the format (ex: h:m;h:m;h:m;h:m) into an array of TimeSpan.
         * Parameters:
         * string line, line read from the csv file
         *
         * Returns:
         * TimeSpan[], a TimeSpan[4] object containing the respective schedule's hours from user input
         *  or from official schedule (in case, it's missing).
         *
         * Throws Exception
         */
        public static TimeSpan[] ParseLine(string line)
        {
            //Removing unnecessary part of the string
            int idx = nIndexOf(line, ';', 4);
            if (idx != -1)
            {
                line = line.Remove(idx);
            }

            //Completing missing cells from line
            for (int counting = line.Count(f => f == ';'); counting < 3; counting++)
            {
                line += ";";
            }

            //Splitting String on ';'
            string[] cells = line.Split(new char[]{';'}, 4, StringSplitOptions.None);

            //Parsing
            TimeSpan[] TimeSpanCell = new TimeSpan[4];
            for(int i = 0; i < cells.Length; i++)
            {
                if(!TimeSpan.TryParseExact(cells[i], format, null, out TimeSpanCell[i]))
                {
                    //Error: parsing failed!
                    TimeSpanCell[i] = officialTS[i];
                }
            }
            return TimeSpanCell;
        }
    }
}
