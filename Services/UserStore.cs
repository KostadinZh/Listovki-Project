using System.Text.Json;
using ListovkiQuiz.Models;

namespace ListovkiQuiz.Services;

/// <summary>
/// Keeps the users and their exam attempts. Everything is saved in two JSON files
/// inside the App_Data folder, so it is still there after the app restarts.
/// </summary>
public class UserStore
{
    // Only one request may change the data at a time
    private readonly object _lock = new object();

    private readonly string _usersFile;
    private readonly string _attemptsFile;
    private readonly List<AppUser> _users;
    private readonly List<Attempt> _attempts;

    public UserStore(IWebHostEnvironment env)
    {
        string folder = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(folder);

        _usersFile = Path.Combine(folder, "users.json");
        _attemptsFile = Path.Combine(folder, "attempts.json");

        _users = Load<AppUser>(_usersFile);
        _attempts = Load<Attempt>(_attemptsFile);
    }

    /// <summary>Creates an account. Returns null if the username is already taken.</summary>
    public AppUser? CreateUser(string username, string password)
    {
        string hash = Passwords.Hash(password);   // slow on purpose, so done before taking the lock

        lock (_lock)
        {
            if (_users.Any(u => SameName(u.Username, username)))
                return null;

            var user = new AppUser
            {
                Id = _users.Count == 0 ? 1 : _users.Max(u => u.Id) + 1,
                Username = username,
                PasswordHash = hash,
                CreatedAt = DateTime.UtcNow
            };
            _users.Add(user);
            Save(_usersFile, _users);
            return user;
        }
    }

    /// <summary>Returns the user if the username and password are right, otherwise null.</summary>
    public AppUser? CheckLogin(string username, string password)
    {
        AppUser? user;
        lock (_lock)
        {
            user = _users.FirstOrDefault(u => SameName(u.Username, username));
        }

        if (user == null)
            return null;

        return Passwords.Verify(password, user.PasswordHash) ? user : null;
    }

    /// <summary>Saves a finished exam. The same exam (same ExamCode) is never saved twice.</summary>
    public void AddAttempt(Attempt attempt)
    {
        lock (_lock)
        {
            if (attempt.ExamCode != "" && _attempts.Any(a => a.ExamCode == attempt.ExamCode))
                return;

            attempt.Id = _attempts.Count == 0 ? 1 : _attempts.Max(a => a.Id) + 1;
            _attempts.Add(attempt);
            Save(_attemptsFile, _attempts);
        }
    }

    /// <summary>All attempts of one user, newest first.</summary>
    public List<Attempt> GetAttempts(int userId)
    {
        lock (_lock)
        {
            return _attempts
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.TakenAt)
                .ToList();
        }
    }

    // Usernames are compared ignoring upper/lower case ("Ivan" and "ivan" are the same person)
    private static bool SameName(string a, string b)
    {
        return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }

    private static List<T> Load<T>(string path)
    {
        if (!File.Exists(path))
            return new List<T>();

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    private static void Save<T>(string path, List<T> items)
    {
        string json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });

        // Write to a temporary file first, then swap it in, so a crash can never leave a half-written file
        string temp = path + ".tmp";
        File.WriteAllText(temp, json);
        File.Move(temp, path, overwrite: true);
    }
}
