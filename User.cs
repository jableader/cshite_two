﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cshite_ass_2
{
    public enum UserType { View, Edit }

    public class User
    {
        const string loginFile = "login.txt";

        public string Username { get; private set; }
        public string Password { get; private set; }
        public UserType Type { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime Birthdate { get; private set; }

        public override string ToString()
            => string.Join(",", new object[] { Username, Password, Type, FirstName, LastName, Birthdate.ToShortDateString() }); // Represent this User as comma seperated values

        static User FromString(string line)
        {
            var items = line.Split(','); // The line contains the user's data as comma seperated values
            return new User // We just need to pluck the values from the array
            {
                Username = items[0],
                Password = items[1],
                Type = Enum.TryParse<UserType>(items[2], out var type) ? type : throw new ArgumentException(nameof(Type)),
                FirstName = items[3],
                LastName = items[4],
                Birthdate = DateTime.TryParse(items[5], out var date) ? date : throw new ArgumentException(nameof(Birthdate))
            };
        }

        /// <summary>
        /// Creates and saves a new user
        /// </summary>
        public static User Create(string username, string password, UserType type, string firstName, string lastName, DateTime birthdate)
        {
            var user = new User { Username = username, Password = password, Type = type, FirstName = firstName, LastName = lastName, Birthdate = birthdate };
            File.AppendAllText(loginFile, "\r\n" + user.ToString()); // Saves this new user to the file

            return user;
        }

        /// <summary>
        /// Returns all users
        /// </summary>
        public static IEnumerable<User> AllUsers
            => File.ReadAllLines(loginFile).Where(s => !string.IsNullOrWhiteSpace(s)).Select(FromString); // Convert each non-empty line into a user

        /// <summary>
        /// Retrieves the user with the assosciated username/password combo or null
        /// </summary>
        /// <returns></returns>
        public static User Authenticate(string username, string password)
            => AllUsers.FirstOrDefault(user => user.Username == username && user.Password == password);
    }
}
