# Unity PlayerPrefs Manager

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Readme_ZH](https://img.shields.io/badge/PlayerPrefsManager-%E4%B8%AD%E6%96%87%E6%96%87%E6%AA%94-red)](https://github.com/barryyip0625/Unity-PlayerPrefs-Manager/blob/main/README_ZH.md)

## Overview

PlayerPrefs Manager is a Unity editor extension tool that allows you to easily view, edit, create, and delete PlayerPrefs data directly within the Unity Editor. This tool is particularly useful for developers who need to manage game settings, user preferences, and save data during development.

![image](https://github.com/user-attachments/assets/90b8c2f9-1b10-4c4d-86e4-20d94e94aea9)

## Features

- View all existing PlayerPrefs with their types and values
- Add new PlayerPrefs entries (String, Int, Float)
- Edit existing PlayerPrefs values and types
- Delete individual PlayerPrefs entries
- Delete all PlayerPrefs at once
- Search functionality to filter PlayerPrefs entries
- Option to display raw keys (with Unity's hash suffix)

## Installation

### Option 1: Using Unity Package

1. Download the latest Unity Package release
2. In Unity Editor, go to Assets > Import Package > Custom Package
3. Select the downloaded package file
4. Make sure all files are selected and click Import

### Option 2: Manual Installation

1. Clone or download this repository
2. Copy the `PlayerPrefsManager.cs` file to the `Assets/Editor` folder in your Unity project
   (create the Editor folder if it doesn't exist)

## Usage

1. In Unity Editor, go to Tools > PlayerPrefsManager
2. The window will display all existing PlayerPrefs for your project
3. Use the search field to filter entries
4. Toggle "Show Raw Keys" to see the hashed versions of keys that Unity uses internally
5. Add new entries using the fields at the top of the window
6. Edit or delete existing entries using the buttons on each row

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Contributing

Issues and pull requests are welcome. Your contributions will help make this tool better!
