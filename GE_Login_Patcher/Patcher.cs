using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace GE_Login_Patcher
{
    class Patcher
    {
        //Lists indices for all occurrences of a key in a string.
        static List<int> IndicesOf(ref string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");

            List<int> indexes = new List<int>();

            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);

                if (index == -1)
                {
                    return indexes;
                }

                indexes.Add(index);
            }
        }

        string[] FindClassNames(ref string data, string key)
        {
            string[] rString = new string[2];

            //Find all mentions of this key in the string.
            List<int> indices = IndicesOf(ref data, key);

            Regex regex = new Regex("^[a-zA-Z]+$"); //Accept only letters as class names

            //Loop trough all entries, find the one that satisfy all conditions.
            for (int i = 0; i < indices.Count; i++)
            {
                //Entries for first condition.
                string class1 = data.ElementAt(indices.ElementAt(i) - 1).ToString();
                string class2 = data.ElementAt(indices.ElementAt(i) - 2).ToString();
                string class3 = data.ElementAt(indices.ElementAt(i) - 3).ToString();

                //First symbol to the left cannot be a letter, next is and one after that is a comma.
                if (!regex.IsMatch(class1) && regex.IsMatch(class2) && class3 == ",")
                {
                    //Entries for our second condition.
                    string classInput1 = data.ElementAt(indices.ElementAt(i) + key.Length + 0).ToString();
                    string classInput2 = data.ElementAt(indices.ElementAt(i) + key.Length + 1).ToString();
                    string classInput3 = data.ElementAt(indices.ElementAt(i) + key.Length + 2).ToString();

                    //Finding input class name is similar, where first match is opening parenthesis, then a letter and a closing parenthesis.
                    if (classInput1 == "(" && regex.IsMatch(classInput2) && classInput3 == ")")
                    {
                        rString[0] = class2;
                        rString[1] = classInput2;

                        //Both found
                        break;
                    }
                }
            }

            return rString;
        }
        void InsertString(ref string data, string keyFirst, string keySecond, int offset, string insert)
        {
            int posStart = data.IndexOf(keyFirst);
            int posSecond = data.IndexOf(keySecond, posStart) + keySecond.Length + offset;

            data = data.Insert(posSecond, insert);
        }

        public
        bool Patch(string path, ref string errorString)
        {
            string filename = "app.js";
            string filenameBackup = "app_BACKUP.js";
            string pathAndFile = path + "\\" + filename;
            string pathAndFileBackup = path + "\\" + filenameBackup;
            string contents = "";

            //Try opening the source file.
            try
            {
                contents = File.ReadAllText(pathAndFile);

                //Check if the file is already patched.
                if (contents.StartsWith("//PATCHED"))
                {
                    errorString = "File has already been patched.";
                }
                else
                {
                    File.Copy(pathAndFile, pathAndFileBackup, true);
                }
            }
            //Catch common exceptions.
            catch (UnauthorizedAccessException)
            {
                FileAttributes attr = (new FileInfo(pathAndFile)).Attributes;
                errorString = "UnAuthorizedAccessException: Unable to access file.";

                if ((attr & FileAttributes.ReadOnly) > 0)
                {
                    errorString = "UnAuthorizedAccessException: The file is read-only.";
                }
            }
            catch (System.Security.SecurityException)
            {
                errorString = "System.Security.SecurityException: Unable to access file.";
            }
            catch (FileNotFoundException)
            {
                errorString = "FileNotFoundException: app.js File not found.";
            }
            catch (DirectoryNotFoundException)
            {
                errorString = "DirectoryNotFoundException: File directory not found.";
            }
            finally
            {
                if (contents == "")
                {
                    errorString = "Unknown exception: Unable to open and read app.js";
                }
            }

            //Exit if error string contains something.
            if (!(errorString == ""))
            {
                return false;
            }



            //Do the actual patching of the file.
            //Find class names used around the handleLoggedIn function, they could potentially change.
            string[] classNames = FindClassNames(ref contents, "handleLoggedIn");

            //None found, can't proceed.
            if (classNames[0] == "" || classNames[1] == "")
            {
                errorString = "Could not locate classes. Source file might have changed too much.";
                return false;
            }

            //Construct a new function call string with the correct class names.
            string completeString = " " + classNames[0] + ".handleLoggedIn(" + classNames[1] + "),";

            //Patch it in.
            InsertString(ref contents, "domains.list.indexOf", "return", 0, completeString);

            //Dummy user string.
            string dummyUserString = "," + classNames[0] + ".handleLoggedIn({sessionToken: \"asd\",userToken: \"asd\",user:{core:{displayName: \"¯∖_(ツ)_/¯\", primaryEmailVerified: true}}});";

            //Patch it in.
            InsertString(ref contents, "isLeftPaneVisible=function()", "}", 0, dummyUserString);

            //Mark the file as patched.
            contents = contents.Insert(0, "//PATCHED\n");

            //Write over the app.js with the new data.
            File.WriteAllText(pathAndFile, contents);

            //Success.
            return true;
        }
    }
}
