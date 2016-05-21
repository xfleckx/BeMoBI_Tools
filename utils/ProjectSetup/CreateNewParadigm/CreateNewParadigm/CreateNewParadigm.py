import os
import urllib
import time
from subprocess import call, check_call, check_output, CalledProcessError
import sys

UNITY_VERSION = '5.3.5f';

GIT_INGORE_FILE = "https://raw.githubusercontent.com/github/gitignore/master/Unity.gitignore"

DEPENDENCIES = {"SNEED" : "https://github.com/xfleckx/SNEED.git",
                "LSL4UNITY" : "https://github.com/xfleckx/LSL4Unity" }

def git_available():
    try:
        output = check_output(["git", "--version"])
        print('Found ' + output)
        return True
    except CalledProcessError as pe: 
        return False
    except  WindowsError as we:
        return False

def try_create_project(pathToProjectRootDirectory):
    createProjectArgument = pathToProjectRootDirectory
    try:
        print("Try creating: " + createProjectArgument) 
        output = check_output(["Unity", "-quit", "-batchmode", "-createProject", pathToProjectRootDirectory])
        os.chdir(pathToProjectRootDirectory)
        return True
    except CalledProcessError as pe: 
        return False
    except WindowsError as we:
        return False

def init_git_repo() :
    call(["git", "init", "--quiet"])
    print("Fetching .gitignore file")
    urllib.urlretrieve(GIT_INGORE_FILE, ".gitignore")
    call(["git", "add", "*"])

def init_submodules():
    os.chdir("Assets")
    print(os.getcwd())

    for key in DEPENDENCIES:
        print("Add Submodule "+key)
        call(["git", "submodule", "add", DEPENDENCIES[key]])

    call(["git", "commit", "-m", "Auto setup finished"])

def open_unity_on(projectName):
    call(["Unity", "-projectPath", projectName], shell=True)

if __name__ == "__main__":
        
    if not git_available():
        print('Please install git first!')
        sys.exit(1)

    projectName = "test"
    pathToNewProject = try_create_project(projectName)

    if not pathToNewProject:
        print("Could not create Unity3D project")
        sys.exit(1)

    init_git_repo()
    init_submodules()
    os.chdir("..")

    print("Setup finished - Open the project")
    
    open_unity_on(projectName)


