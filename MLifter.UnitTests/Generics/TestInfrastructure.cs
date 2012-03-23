/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace MLifterTest.Generics
{
    public static class TestInfrastructure
    {
        /// <summary>
        /// Gets the random string.
        /// </summary>
        /// <param name="Length">The length of the string.</param>
        /// <returns>A random string.</returns>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        public static string GetRandomString(int Length)
        {
            return GetRandomString(Length, true, true);
        }

        /// <summary>
        /// Gets a random string.
        /// </summary>
        /// <param name="Length">The length of the string.</param>
        /// <param name="IncludeSpecialCharacters">if set to <c>true</c> [include special characters].</param>
        /// <param name="IncludeNumbers">if set to <c>true</c> [include numbers].</param>
        /// <returns>A random string.</returns>
        public static string GetRandomString(int Length, bool IncludeSpecialCharacters, bool IncludeNumbers)
        {
            string returnval = "";
            for (int i = 1; i <= Length; i++)
            {
                returnval += GetRandomChar(IncludeSpecialCharacters, IncludeNumbers).ToString();
            }
            return returnval;
        }

        /// <summary>
        /// Gets a Random Character
        /// </summary>
        /// <param name="IncludeSpecialCharacters">if set to <c>true</c> [include special characters].</param>
        /// <param name="IncludeNumbers">if set to <c>true</c> [include numbers].</param>
        /// <returns>A random char</returns>
        private static char GetRandomChar(bool IncludeSpecialCharacters, bool IncludeNumbers)
        {
            //33-47 , 58-64, 91-96, 123-126 special characters
            //48-57 numbers
            //65-90 uppercase letters
            //97-122 lowercase letters
            bool GetNewChar = true;

            int randint = 0;
            if (IncludeNumbers && IncludeSpecialCharacters)
            {
                randint = RandomGen.Next(33, 122);
            }
            else if (IncludeNumbers)
            {
                //get a number between  48 and 122 but not between 58 and 64
                while (GetNewChar)
                {
                    randint = RandomGen.Next(48, 122);
                    GetNewChar = (randint >= 58 && randint <= 64);
                }
            }
            else if (IncludeSpecialCharacters)
            {
                while (GetNewChar)
                {
                    randint = RandomGen.Next(33, 122);
                    GetNewChar = (randint >= 48 && randint <= 57);
                }
            }
            else
            {
                while (GetNewChar)
                {
                    randint = RandomGen.Next(65, 122);
                    GetNewChar = (randint >= 91 && randint <= 96);
                }
            }
            return Convert.ToChar(char.ConvertFromUtf32(randint));
        }

        private static System.Random _RandomGen;
        public static System.Random RandomGen
        {
            get
            {
                if (_RandomGen == null)
                {
                    _RandomGen = new System.Random();
                }
                return _RandomGen;
            }
        }
    }
}
