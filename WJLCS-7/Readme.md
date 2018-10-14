# WJLCS-7

* **Author:** Robert Jordan
* **Language:** C# (.NET Framework 4.6.2)
* **Requires:** Visual Studio 2017

## What have I learned this week in programming

This week (and last week), I spent the majority of the time conforming my existing library to match the given specifications. I've learned that conforming to specifications can be quite tricky and cumbersome especially when there's a million different ways to implement the specification.

I've also determined that when I actually start working, that I'll need to work on deciphering vague wording to determine what my team actually needs.

## How I ended up Interpreting the Specifications

The Interpreter class was basically up to interpretation on exactly how much it did and didn't do. It was also unclear if the seperate objects for each menu included every menu, or just enciphering screens. I interpreted it as only menus that interacted with the workings of the Enigma Machine. So choice-based menus ended up being excluded from it.

I wasn't sure how much code should be in the interpreter class, so I made the interpreter class handle I/O for the interpreter-based menus while the contained objects handled interacting with the Enigma Machine Library directly. This conforms to a View/Model/ViewModel sort of approach where the Interpreter is the View Model, all the Enigma Machine handling classes are the Models, and the Menus and MenuDriver are the actual Views. The Enigma Machine library itself is managed inside of the Models and is not directly used by anything but them.

I also had to guess if the method and class naming were supposed to be exactly as written in the specifications but I determined that was wrong because all the class names listed had `Class` at the end of their name, and that's a pretty strange convention. It seemed to be that the methods/class names were just named to be informative. On the other hand. I did give the methods and classes similar names to what was listed in the specification so that they could easily be identified. Although I admit, I was nota fan of renaming the `Menu` and `Screen` classes to `GetMenu` and `GetScreen`.

So I've really got it drilled into me how wide of a way specifications can be interpreted in.

### Video Presentation: [Link](https://www.youtube.com/watch?v=OoEn1ZfVi9M)
