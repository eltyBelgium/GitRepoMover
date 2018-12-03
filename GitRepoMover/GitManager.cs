using LibGit2Sharp;
using System;
using System.IO;
using System.Linq;

namespace GitRepoMover
{
    public class GitManager
    {
        private readonly string _username;
        private readonly string _password;
        private string _path;
        private readonly bool _allbranches;
        private Branch _currentBranch;

        public GitManager(string username, string password, string path, bool allbranches)
        {
            _username = username;
            _password = password;
            _path = path;
            _allbranches = allbranches;
        }


        public void GitClone(string repo)
        {
            try

            {
                DeleteDirectory(_path);

                var co = new CloneOptions
                {
                    CredentialsProvider = (_url, _user, _cred) =>
                        new UsernamePasswordCredentials { Username = _username, Password = _password }
                };
                _path = Repository.Clone(repo, _path, co);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void GitGetAllBranches()
        {
            try
            {
                using (var repo = new Repository(_path))
                {
                    var branches = repo.Branches.Where(b => b.IsRemote).ToList();

                    foreach (var branch in branches)
                    {
                        Console.WriteLine($"getting branch {branch.FriendlyName}");
                        _currentBranch = Commands.Checkout(repo, branch);
                    }

                    var masterBranch = repo.Branches.FirstOrDefault(b => b.RemoteName == "origin");
                    _currentBranch = Commands.Checkout(repo, masterBranch);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void GitMoveRepo(string newRepoUrl)
        {
            try
            {
                using (var repo = new Repository(_path))
                {
                    repo.Network.Remotes.Update("origin", r => r.Url = newRepoUrl);

                    Remote remote = repo.Network.Remotes["origin"];

                    var options = new PushOptions();
                    options.CredentialsProvider = (_url, _user, _cred) =>
                        new UsernamePasswordCredentials { Username = _username, Password = _password };


                    repo.Network.Push(remote, @"refs/heads/master", options);

                    if (_allbranches)
                    {
                        var branches = repo.Branches.Where(b => b.IsRemote).ToList();

                        foreach (var branch in branches)
                        {
                            _currentBranch = Commands.Checkout(repo, branch);

                            repo.Branches.Update(branch,
                                b => b.Remote = remote.Name,
                                b => b.UpstreamBranch = branch.CanonicalName);

                            repo.Network.Push(branch, options);

                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void DeleteDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            var files = Directory.GetFiles(directoryPath);
            var directories = Directory.GetDirectories(directoryPath);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in directories)
            {
                DeleteDirectory(dir);
            }

            File.SetAttributes(directoryPath, FileAttributes.Normal);

            Directory.Delete(directoryPath, false);
        }
    }
}