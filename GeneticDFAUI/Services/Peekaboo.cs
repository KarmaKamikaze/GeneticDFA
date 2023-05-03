using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace GeneticDFAUI.Services;

public delegate void FileCreationEvent(List<string> fileNames);

/// <summary>
/// A file watcher that will detect changes in a folder. Inspiration for this class comes from FileSystemWatcher.
/// </summary>
public class Peekaboo
{
    private Timer _timer;
    private string _directory;
    private FileCreationEvent? _onFileCreationHandler;
    private readonly List<string> _fileNames = new List<string>();

    private enum HandlerTypes
    {
        Created = 1,
        Deleted = 2,
        Changed = 3,
        Renamed = 4,
        All = Created | Deleted | Changed | Renamed
    }

    public Peekaboo()
    {
        _directory = string.Empty;
    }

    public Peekaboo(string path)
    {
        CheckPathValidity(path);
        _directory = path;
    }

    public Peekaboo(string path, string filter) : this(path)
    {
        Filter = filter;
    }

    public event FileCreationEvent? FileCreated
    {
        add => _onFileCreationHandler += value;
        remove => _onFileCreationHandler -= value;
    }

    public string Filter { get; set; } = "*.*";

    public string Path
    {
        get => _directory;
        set
        {
            value = (value == null) ? string.Empty : value;
            if (!string.Equals(_directory, value, StringComparison.Ordinal))
            {
                CheckPathValidity(value);
                _directory = value;
            }
        }
    }

    public bool IncludeSubDirectories { get; set; } = false;

    private static void CheckPathValidity(string path)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));

        // Early check for directory parameter so that an exception can be thrown as early as possible.
        if (path.Length == 0)
            throw new ArgumentException("No directory parameter has been set", nameof(path));

        if (!Directory.Exists(path))
            throw new ArgumentException($"The following path does not exist: {path}", nameof(path));
    }

    private FileCreationEvent? GetHandler(HandlerTypes handlerType)
    {
        switch (handlerType)
        {
            case HandlerTypes.Created:
                return _onFileCreationHandler;
        }

        return null;
    }

    private void OnFilesChanged(List<string> fileNames, FileCreationEvent? handler)
    {
        if (handler != null)
        {
            handler.Invoke(fileNames);
        }
    }

    private bool ScanDirectory()
    {
        SearchOption searchOption = IncludeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        List<string> filesInDirectory = Directory.GetFiles(_directory, Filter, searchOption).ToList();

        bool newFiles = false;
        foreach (string file in filesInDirectory)
        {
            string fileName = System.IO.Path.GetFileName(file).Split('.')[0];
            if (!_fileNames.Contains(fileName))
            {
                newFiles = true;
                _fileNames.Add(fileName);
            }
        }

        return newFiles;
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        bool newFiles = ScanDirectory();
        if (newFiles)
        {
            OnFilesChanged(_fileNames, GetHandler(HandlerTypes.Created));
        }
    }

    public void StartScanning(int intervalInMilliseconds)
    {
        _timer = new Timer(intervalInMilliseconds);
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
        _timer.Start();
    }
}
