# Scriptable Object Browser
Are you tired from trying to find scriptable objects inside messy folders?
Then, you came to the right door.

SO Browser lists all ScriptableObject classes that exist in your project for you.
You can list any ScriptableObject class and view/edit/clone any instance of it!

# Usage
- Open the SO Browser from ```Tools > ScriptableObject Browser```
- Select any ScriptableObject class
- View/Edit/Clone!
<br></br>
- Go to directory of any instance from "Go to directory" button
- Clone any instance from "Create [classname] Clone" button

![sobrowser_preview](https://github.com/user-attachments/assets/fdc7bd43-e6de-4d7e-ab37-31676225a7c4)

# Config

SO Browser ignores default namespaces of Unity (UnityEngine and UnityEditor) to avoid unnecessary classes in the menu.
You can edit ignored namespaces from:<br> ```SO Browser Home Page > Config Button > Ignored Namespaces Textbox``` <br>
- Write each namespace in a new line.

![Screenshot_18](https://github.com/user-attachments/assets/f46104cf-119f-4670-9bfa-e73a03a27092)

# Limitations
- There is no way to detect classes that don't have a ScriptableObject on the project folder.
You need to have at least 1 instance of that class to access it through the SO Browser.

- It's impossible to create a file without knowing its class before compiling.
So, it can't create an empty ScriptableObject but copy the selected one.

# Installation

### 1. Install with UPM<br>
- Open the Unity Package Manager
- Select ```Install package from git URL```
- Copy and paste: ```https://github.com/NotGeniusy/sobrowser.git```<br>
- Done!<br>
![Screenshot_21](https://github.com/user-attachments/assets/0af3c340-6d7e-4543-a574-471351f3bb4a)

### 2. Manual installation with .zip
- Download the .zip from GitHub ```https://github.com/NotGeniusy/sobrowser/archive/refs/heads/main.zip```
- Extract the .zip to ```[Project Directory]/Packages```

# Requirements
```Unity 2019.1 or higher``` <br>
It uses UnityEngine.UIElements that released in Unity 2019.1
