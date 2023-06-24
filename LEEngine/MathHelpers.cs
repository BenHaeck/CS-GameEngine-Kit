using System.Numerics;



public class MathHelpers {
	// takes in the size of the canvas, then calculates
	// a position and scale that fits the entire thing in a window
	public static (Vector2, float) FitCanvas (Vector2 canvasSize, Vector2 windowSize) {
		if (canvasSize == Vector2.Zero || windowSize == Vector2.Zero)
			return (Vector2.Zero, 1);

		// defines the variables
		Vector2 position = Vector2.Zero;
		float scale = 1;
		float canvAspR = canvasSize.Y / canvasSize.X;
		float winAspR = windowSize.Y / windowSize.X;

		
		if (canvAspR < winAspR){
			// fills the x axis
			scale = windowSize.X / canvasSize.X;
			position.Y = (windowSize.Y - canvasSize.Y * scale)*0.5f;
			
		} else {
			// fills the y axis
			scale =  windowSize.Y / canvasSize.Y;
			position.X = (windowSize.X - canvasSize.X * scale)*0.5f;

		}
		
		return (position, scale);
	}

	// takes in the position and scale of the canvas, then converts
	// the position to a canvas position
	public static Vector2 ScreenToCanvas ((Vector2 pos, float scale) canvasVals, Vector2 point) {
		return (point - canvasVals.pos) / canvasVals.scale;
	}
}