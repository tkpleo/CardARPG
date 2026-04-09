# Independent Research Project
Our independent research project was an experiment to make a game that combined the real time combat of an isometric action roguelike like Hades or Enter the Gungeon with the deckbuilding mechanics of deckbuilding roguelikes.
The player plays cards in real time to shoot bullets from their revolver as they fight enemies. The player has a deck of cards they currate and manipulate after combat encounters as they try to develop a strategy against the every increasing difficulty of enemies.
# Protfolio Overview
There are only two programmers on this project, including me. There are many scripts that are completely my own work including the following:
1. Assets/Scripts/Bullet Systems/Bullet Behavior and Bullet Pooler. These work on the shooting aspect of the game while setting up a bullet pooler for performance.
2. Assets/Scripts/Script Pattern Declaration/Object Pooler sets up an abstract object pooler to be used for the bullets and for enemies.
3. Assets/Scripts/Card Mechanics holds the card instance and deck manager which manage the player's deck in and out of combat.
4. Assets/Scripts/SO Declarations contains numerous ScriptableObjects that I made. Bullet Data works with the bullet system to easily create different kinds of bullets that the player and enemies can shoot.
5. Assets/Scripts/SO Declarations/Cards contains the card specific scriptable objects which help quickly implement the card pool in the game. 
