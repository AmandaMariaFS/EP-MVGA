uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform float iTime;
uniform vec2 iResolution;
uniform vec4 iMouse;
#define EPS  .01
#define COL0 vec3(.2, .35, .55)
#define COL1 vec3(.9, .43, .34)
#define COL2 vec3(.96, .66, .13)
#define COL3 vec3(0.0)
#define COL4 vec3(0.99,0.1,0.09)




/*  Install  Istructions

sudo apt-get install g++ cmake git
 sudo apt-get install libsoil-dev libglm-dev libassimp-dev libglew-dev libglfw3-dev libxinerama-dev libxcursor-dev
libxi-dev libfreetype-dev libgl1-mesa-dev xorg-dev

git clone https://github.com/JoeyDeVries/LearnOpenGL.git*/

float df_circ(in vec2 p, in vec2 c, in float r)
{
    return abs(r - length(p - c));
}


// Visual line for debugging purposes.
bool line (vec2 p, vec2 a, vec2 b, float bary)
{
    // Direction from a to b.
    vec2 ab = normalize(b - a);

    // Direction from a to the pixel.
    vec2 ap = p - a;

    float thickness = 0.0025;

    if(bary < 0.){
        thickness += abs(bary/10);
    }

    // Find the intersection of the pixel on to vector
    // from a to b, calculate the distance between the
    // pixel and the intersection point, then compare
    // that distance to the line width.
    return length((a + ab * dot(ab, ap)) - p) < thickness;
}

vec3 bary(in vec3 a, in vec3 b, in vec3 c, in vec3 p)
{
    float beta = (c.y - a.y - ((a.x - c.x) * (a.y - b.y)) / (b.x - a.x));
    float alphaC = (((p.x - a.x) * (a.y - b.y)) / (b.x - a.x) + p.y - a.y) / beta;
    float alphaB = (p.x - a.x + alphaC * (a.x - c.x)) / (b.x - a.x);
    float alphaA = 1.0 - alphaB - alphaC;

    return vec3(alphaA, alphaB, alphaC);
}

bool test(in vec2 a, in vec2 b, in vec2 c, in vec2 p, inout vec3 barycoords)
{
    barycoords = bary(vec3(a.x, a.y, 0.),
                  vec3(b.x, b.y, 0.),
                  vec3(c.x, c.y, 0.),
                  vec3(p.x, p.y, 0.));

    return barycoords.x > 0. && barycoords.y > 0. && barycoords.z > 0.;
}

void main()
{
    float ar = iResolution.x / iResolution.y;
        vec2 mc=vec2(0.0);
        vec2 uv = (gl_FragCoord.xy / iResolution.xy * 2. - 1.) * vec2(ar, 1.);
        mc = (iMouse.xy    / iResolution.xy * 2. - 1.) * vec2(ar, 1.);


    vec2 a = vec2( .73,  .75);
    vec2 b = vec2(-.85,  .15);
    vec2 c = vec2( .25, -.75);
    vec3 barycoords;

    bool t0 = test(a, b, c, mc,barycoords);
    vec3 color=vec3(0.0);
    vec3 colorPink=vec3(0.5, 1.0, 0.6);
    // Visual debug lines and points.
       if (line(uv, a, b, barycoords.z))
           color = vec3(0.0, 1.0, 0.0);
       if (line(uv, b, c, barycoords.x))
           color = vec3(0.4, 1.0, 0.0);
       if (line(uv, c, a, barycoords.y))
           color = vec3(0.0, 1.0, 0.4);
       if (df_circ(uv, a,EPS)<0.5*EPS)
          color = vec3(0.0, 1.0, 0.0);
      if (df_circ(uv, b,EPS)<0.5*EPS)
          color = vec3(1.0, 0.0, 0.0);
      if (df_circ(uv, c,EPS)<0.5*EPS)
          color = vec3(0.0, 0.0, 1.0);

    vec3 col = t0 ? colorPink-color : color;
        gl_FragColor = vec4(1.0-col, 1);
}
