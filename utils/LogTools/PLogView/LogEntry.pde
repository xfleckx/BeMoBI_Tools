import java.util.regex.Pattern;

public class LogEntry{
  
  PFont font;
  
  public LogEntry(){
   
  // The font must be located in the sketch's 
  // "data" directory to load successfully
    font = loadFont("Lato-Regular-18.vlw"); 
  }

  
  public String content;
  
  public color textColor = color(255,255,255);
  public float fontSize = 14;
  public color backgroundColor;
  
  public void render(PVector pos, float w, float h){
    
    pushMatrix();
    pushStyle();
    
    translate(pos.x, pos.y);
    
    rect(0, h, 0, h);
    
    fill(textColor);
    textFont(font, 32);
    textSize(fontSize);
    rectMode(CORNERS);
    
    text(content, 0, 0 + fontSize);
    
    popStyle();
    popMatrix();
    
  }

}