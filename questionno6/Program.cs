using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        // Part 1: Username Validation
        Console.WriteLine("Enter usernames (separated by commas): ");
        string input = Console.ReadLine();
        if (input == null) return;  // Handle null input

        string[] usernames = input.Split(',');

        List<string> invalidUsernames = new List<string>();
        int validCount = 0;
        int invalidCount = 0;

        var results = new List<string>();

        // Updated pattern to enforce underscores only at the start or end
        string pattern = @"^[a-zA-Z0-9]+(_[a-zA-Z0-9]+)*$";

        foreach (var username in usernames)
        {
            string trimmedUsername = username.Trim();
            bool isValid = true;
            string reason = string.Empty;

            // Username regex validation
            if (!Regex.IsMatch(trimmedUsername, pattern))
            {
                isValid = false;
                if (trimmedUsername.Length < 5 || trimmedUsername.Length > 15)
                    reason = "Username length must be between 5 and 15 characters.";
                else if (!char.IsLetter(trimmedUsername[0]))
                    reason = "Username must start with a letter.";
                else
                    reason = "Username can only contain letters, numbers, and underscores, and underscores can only be at the start or end.";
            }

            if (isValid)
            {
                validCount++;
                results.Add($"Validation Results: \n{trimmedUsername} - Valid");

                int upperCount = 0, lowerCount = 0, digitCount = 0, underscoreCount = 0;
                foreach (var ch in trimmedUsername)
                {
                    if (char.IsUpper(ch)) upperCount++;
                    else if (char.IsLower(ch)) lowerCount++;
                    else if (ch == '_') underscoreCount++;
                    else if (char.IsDigit(ch)) digitCount++;
                }

                results.Add($"Letters: {trimmedUsername.Length} (Uppercase: {upperCount}, Lowercase: {lowerCount}), Digits: {digitCount}, Underscores: {underscoreCount}");

                // Generate password and check strength
                string password = GeneratePassword();
                string passwordStrength = EvaluatePasswordStrength(password);
                results.Add($"Generated Password: {password} (Strength: {passwordStrength})\n");
            }
            else
            {
                invalidCount++;
                invalidUsernames.Add(trimmedUsername);
                results.Add($"{trimmedUsername} - Invalid ({reason})\n");
            }
        }

        // Display results in console
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }

        // Part 2: Save Results to File
        SaveResultsToFile(results, validCount, invalidCount, usernames.Length, invalidUsernames);

        // Part 3: Retry Invalid Usernames
        if (invalidCount > 0)
        {
            Console.WriteLine("Do you want to retry invalid usernames? (y/n): ");
            string retryResponse = Console.ReadLine();
            if (retryResponse?.ToLower() == "y")
            {
                Console.WriteLine("Enter invalid usernames: ");
                string retryInput = Console.ReadLine();
                if (retryInput != null)
                {
                    string[] retryUsernames = retryInput.Split(',');

                    foreach (var username in retryUsernames)
                    {
                        string trimmedUsername = username.Trim();
                        Console.WriteLine($"Retrying validation for {trimmedUsername}...");
                        // Perform the same validation logic here...
                        bool retryValid = true;
                        string retryReason = string.Empty;
                        if (!Regex.IsMatch(trimmedUsername, pattern))
                        {
                            retryValid = false;
                            if (trimmedUsername.Length < 5 || trimmedUsername.Length > 15)
                                retryReason = "Username length must be between 5 and 15 characters.";
                            else if (!char.IsLetter(trimmedUsername[0]))
                                retryReason = "Username must start with a letter.";
                            else
                                retryReason = "Username can only contain letters, numbers, and underscores, and underscores can only be at the start or end.";
                        }
                        if (retryValid)
                        {
                            Console.WriteLine($"{trimmedUsername} is valid.");
                        }
                        else
                        {
                            Console.WriteLine($"{trimmedUsername} is invalid: {retryReason}");
                        }
                    }
                }
            }
        }

        Console.WriteLine("Processing complete.");
    }

    static string GeneratePassword()
    {
        var random = new Random();
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*";

        char[] password = new char[12];
        password[0] = upper[random.Next(upper.Length)];
        password[1] = upper[random.Next(upper.Length)];
        password[2] = lower[random.Next(lower.Length)];
        password[3] = lower[random.Next(lower.Length)];
        password[4] = digits[random.Next(digits.Length)];
        password[5] = digits[random.Next(digits.Length)];
        password[6] = specialChars[random.Next(specialChars.Length)];
        password[7] = specialChars[random.Next(specialChars.Length)];

        for (int i = 8; i < 12; i++)
        {
            string allChars = upper + lower + digits + specialChars;
            password[i] = allChars[random.Next(allChars.Length)];
        }

        return new string(password);
    }

    static string EvaluatePasswordStrength(string password)
    {
        int score = 0;
        if (password.Length >= 12) score++;
        if (Regex.IsMatch(password, @"[A-Z]")) score++;
        if (Regex.IsMatch(password, @"[a-z]")) score++;
        if (Regex.IsMatch(password, @"[0-9]")) score++;
        if (Regex.IsMatch(password, @"[!@#$%^&*]")) score++;

        if (score == 5) return "Strong";
        else if (score >= 3) return "Medium";
        else return "Weak";
    }

    static void SaveResultsToFile(List<string> results, int validCount, int invalidCount, int totalCount, List<string> invalidUsernames)
    {
        using (StreamWriter writer = new StreamWriter("UserDetails.txt"))
        {
            foreach (var result in results)
            {
                writer.WriteLine(result);
            }

            writer.WriteLine($"Summary:\n- Total Usernames: {totalCount}\n- Valid Usernames: {validCount}\n- Invalid Usernames: {invalidCount}\n");

            if (invalidUsernames.Count > 0)
            {
                writer.WriteLine("Invalid Usernames:");
                foreach (var invalidUsername in invalidUsernames)
                {
                    writer.WriteLine(invalidUsername);
                }
            }
        }
    }
}
