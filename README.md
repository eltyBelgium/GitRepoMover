# GitRepoMover

# What does it do

Moving a configured repo from 1 location to another one. example: from Github to VSTS.

# How does it work?

1.  you pull the master branch
2.  you make a copy of config.synced.json and rename it to config.synced.json
3.  then you change the values in the config. 

    { 
		
    "CurrentUrl": "", // the current url of the repository
		
    "NewUrl": "", // the destination url of the repository
		
    "Username": "", // your credentials to acces the urls
		
    "Password": "", // your credentials to acces the urls
		
    "LocalTempFolder": "", // your local folder where the repo can be cloned. Will be deleted at the end 
		
    "AllBranches": true // false = only the master branch , true = all branches in this repo.
		
    }

4.  build and let it run.

# This project uses the following 3th party's

thanks to LibGit2Sharp, git
