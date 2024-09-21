# Scriptable Object Browser
Are you tired from trying to find scriptable objects inside messy folders?
Then, you came to the right door.

SO Browser lists all ScriptableObject classes that exists in your project for you.
You can list any ScriptableObject class and view/edit/clone any instance of it!

# Usage
- Open the SO Browser from Tools > ScriptableObject Browser
- Select any ScriptableObject class
- View/Edit/Clone!
<br></br>
- Go to directory of any instance from "Go to directory" button
- Clone any instance from "Create [classname] Clone" button

# Limitations
- There is no way to detect classes that don't have a ScriptableObject on the project folder.
You need to have at least 1 instance of that class to access it through the SO Browser.

- It's impossible to create a file without knowing its class before compiling.
So, it can't create a empty ScriptableObject but copy the selected one.

# Config

SO Browser ignores default namespaces of Unity (UnityEngine and UnityEditor) to avoid unnecessary classes in the menu.
You can edit ignored namespaces from Packages > ScriptableObject Browser > Editor > Config
