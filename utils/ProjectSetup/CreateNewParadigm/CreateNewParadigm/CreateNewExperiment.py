import os
import urllib
import time
from subprocess import call, check_call, check_output, CalledProcessError
import sys
import argparse

UNITY_VERSION = '5.3.5f';

GIT_INGORE_FILE = "https://raw.githubusercontent.com/github/gitignore/master/Unity.gitignore"

DEPENDENCIES = { 
	"VREF" : "https://github.com/xfleckx/VREF",
	"SNEED" : "https://github.com/xfleckx/SNEED.git",
	"LSL4UNITY" : "https://github.com/xfleckx/LSL4Unity"
	}


parser = argparse.ArgumentParser(description='Create a new Unity3D project containing the VREF components')

parser.add_argument('projectName', help="The name of the project")

parser.add_argument('--use_submodules', help="setup the framework components as submodules")

parser.add_argument('--nogit', help="setup the framework components as submodules")

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

def init_framework_components_as_submodules():
	os.chdir("Assets")
	print(os.getcwd())

	for key in DEPENDENCIES:
		print("Add Submodule "+key)
		call(["git", "submodule", "add", DEPENDENCIES[key]])

	call(["git", "commit", "-m", "Auto setup finished"])

def init_framework_components_as_git_repos():
	os.chdir("Assets")
	print(os.getcwd())

	for name in DEPENDENCIES:
		print("clone " + name)
		call(["git", "clone", DEPENDENCIES[name]])

def open_unity_on(projectName):
	call(["Unity", "-projectPath", projectName], shell=True)

if __name__ == "__main__":
	
	if not git_available():
		print('Please install git first!')
		sys.exit(1)
	
	args = parser.parse_args();

	projectName = args.projectName
	pathToNewProject = try_create_project(projectName)

	if not pathToNewProject:
		print("Could not create Unity3D project")
		sys.exit(1)

	if not args.nogit:
		init_git_repo()

	if (not args.nogit) and args.use_submodules:
		init_framework_components_as_submodules()
	else:
		init_framework_components_as_git_repos()

	os.chdir("..")

	print("Setup finished - Open the project")
	
	open_unity_on(projectName)