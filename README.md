<h1 align='center'>
<img src='/ReadmeAssets/GameOfLifeTitle.png' alt='Title Game Of Life'/>
</h1>

<h3 align='center'>
Game of life project for school
</h3>

<p align='center'>
<img src='/ReadmeAssets/GameOfLifeScreenShot.png'/>
</p>

# Summary
* [Requirements](#requirements)
* [Quickstart](#quickstart)
* [Project overview](#projectOverview)
* [Project features](#projectFeatures)
* [Remarks](#remarks)
* [Play Recommendations](#playRecommandations)


## <a name='requirements'>Requirements</a>

Unity version 2020.3.19f1

C# version 8.0

*Note: This is the version used in school computers, it may run on lower one*

## <a name='quickstart'>Quickstart</a>

command:

```bash
$ git clone "https://github.com/LeoKaiser2000/Unity-GameOfLife.git"
```
Clone the project, open unity, add project, and open it.

## <a name='projectOverview'>Project overview</a>

This project is a Conway's Game of Life made in unity for school.

Wikipedia:

`
The Game of Life, also known simply as Life, is a cellular automaton devised by the British mathematician John Horton Conway in 1970. It is a zero-player game, meaning that its evolution is determined by its initial state, requiring no further input. One interacts with the Game of Life by creating an initial configuration and observing how it evolves. It is Turing complete and can simulate a universal constructor or any other Turing machine.
`

## <a name='projectFeatures'>Project features</a>

### Mandatory

* Functionnal Game of Life ✓
* Cell colors
    * colored alive cells ✓
    * undisplayed dead cells ✓
* Random initial pattern ✓
* Restart button  ✓ *(No real restart button but you can use randomize or fill button for restart)*
* Clear button ✓
* Drawing with mouse ✓
* Predifined shapes that can be drag and drop ✓
* Resizable grid ✓

### Bonus

* Bitmap loading ✓

### Other features that wasn't asked

* Default color selection ✓
* Random rate selection ✓
* Game mode based on color mode
    * Random ✓
    * Migration ✓
    * Rainbow ✓

The Random color mode generate a random color for each new cell.

The Migration color mode select new cell color based on highter number of same parents (if no parent, default color is used).

The Rainbow color mode select new cell color based on average of same color (if no parent, default color is used).

## <a name='remarks'>Remarks</a>

### Grid size

The minimum grid size is 4x4.

The maximum grid size is 300x300

### Bitmap dead cells color

For bitmap loading, a pixel is alive if is apha color is less than 100 and if one or more of the red, green or blue colors are highter than 5 (transparent or black pixels are dead).

### Bitmap library

As I made the project on a Linux, Bitmap C# library was not provided. To avoid the problem I used a Unity Script (BMPLoader.cs) That I did not made myself.


## <a name='playRecommandations'>Play Recommendations</a>

### Create awesome Migration/Rainbow games

Select the size you want (100x100 is a good choice)

Select the random rate you want (~20% is optimal)

Select color mode Random

Generate random

Select Migration or Rainbow color mode

Start your game !!!
