# DOTS-MandelbrotPlotter

Plotting mandelbrot set using Unity DOTS

Plots a Mandelbrot set using unity DOTS.
Leverages Unity Jobs to calculate and plot a Mandelbrot set in a highly performant way.
All the points are rendered into a texture and can be configured inside the MandelbrotPlotter Monobehaviour.
![image](https://user-images.githubusercontent.com/5209751/147308181-56a0beae-ade1-4c58-9a57-d19de13fa053.png)

### Instruction

- Press any key or click anywhere to plot Mandelbrot set
- Left click to zoom in
- Right click to zoom out
- Press R to reset viewport

Profiling of the Mandelbrot execution is displayed inside the viewport.

#### TODO:
Use pure DOTS. Right now the "hard" world is done using jobs and burst compiler which translates to insanely fast iteration times, but a pure DOTS would be more elegant for the future.
