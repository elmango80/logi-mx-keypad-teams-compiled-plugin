#!/usr/bin/env python3
# Genera un banner wireframe 3D (CAD/neon) del MX Keypad, dibujado a mano en SVG.
# Reconstruir readme-header.png (ejecutar dentro de assets/):
#   python3 readme-header.gen.py            # escribe ./readme-header.svg
#   "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome" --headless=new \
#     --disable-gpu --force-device-scale-factor=2 --default-background-color=00000000 \
#     --window-size=1280,480 --screenshot=readme-header.png "file://$PWD/readme-header.svg"
import math

A = math.radians(30)
COS, SIN = math.cos(A), math.sin(A)

def proj(p):
    x, y, z = p
    return ((x - y) * COS, (x + y) * SIN - z)

# ---- modelo 3D (unidades arbitrarias) ----
def box(x0, y0, z0, x1, y1, z1):
    v = [(x0,y0,z0),(x1,y0,z0),(x1,y1,z0),(x0,y1,z0),
         (x0,y0,z1),(x1,y0,z1),(x1,y1,z1),(x0,y1,z1)]
    e = [(0,1),(1,2),(2,3),(3,0),(4,5),(5,6),(6,7),(7,4),(0,4),(1,5),(2,6),(3,7)]
    return v, e

def top_face(x0, y0, x1, y1, z):
    v = [(x0,y0,z),(x1,y0,z),(x1,y1,z),(x0,y1,z)]
    return v, [(0,1),(1,2),(2,3),(3,0)]

segments = []          # (a3d, b3d, kind)
nodes = []             # 3d points to mark
def add_box(*a, kind="key"):
    v, e = box(*a)
    for (i, j) in e:
        segments.append((v[i], v[j], kind))
    return v

# base (lip) y cuerpo superior
base_v = add_box(-8,-10,0, 98,128,8, kind="body")
add_box(0,0,8, 90,118,16, kind="body")

# rejilla 3x3 de teclas (z 16..25)
cell, gap, kh = 22, 6, 9.0
cols = [6, 6+cell+gap, 6+2*(cell+gap)]      # 6, 34, 62
rows = [32, 32+cell+gap, 32+2*(cell+gap)]   # 32, 60, 88
hi_mag = (1,1)   # tecla central -> magenta (mic)
hi_grn = (2,0)   # arriba-izq     -> verde (camara)
for ri, v0 in enumerate(rows):
    for ci, u0 in enumerate(cols):
        kind = "key"
        if (ri,ci) == hi_mag: kind = "mag"
        elif (ri,ci) == hi_grn: kind = "grn"
        vs = add_box(u0, v0, 16, u0+cell, v0+cell, 16+kh, kind=kind)
        # nodos en las 4 esquinas superiores
        for idx in (4,5,6,7):
            nodes.append(vs[idx])

# teclas de navegacion (frente)
add_box(10, 8, 16, 40, 24, 22, kind="key")
add_box(46, 8, 16, 78, 24, 22, kind="key")

# cable (curva)
cable_pts = [(90,118,14),(112,150,20),(150,168,30),(205,172,34)]

# rejilla de suelo (z=0)
floor = []
for gx in range(-40, 150, 18):
    floor.append(((gx,-40,0),(gx,150,0)))
for gy in range(-40, 150, 18):
    floor.append(((-40,gy,0),(150,gy,0)))

# ---- ajuste a lienzo ----
allpts = []
for (a,b,_) in segments: allpts += [a,b]
allpts += base_v
pj = [proj(p) for p in allpts]
minx = min(p[0] for p in pj); maxx = max(p[0] for p in pj)
miny = min(p[1] for p in pj); maxy = max(p[1] for p in pj)
TARGET_H = 348.0
fit = TARGET_H / (maxy - miny)
cx, cy = 922, 248
ox = cx - (minx+maxx)/2*fit
oy = cy - (miny+maxy)/2*fit
def place(p):
    sx, sy = proj(p)
    return (sx*fit+ox, sy*fit+oy)

def line(a, b, cls):
    (x1,y1) = place(a); (x2,y2) = place(b)
    return f'<line x1="{x1:.1f}" y1="{y1:.1f}" x2="{x2:.1f}" y2="{y2:.1f}" class="{cls}"/>'

# ---- construir SVG ----
out = []
out.append('<svg xmlns="http://www.w3.org/2000/svg" width="1280" height="480" viewBox="0 0 1280 480">')
out.append('''<defs>
  <linearGradient id="bg" x1="0" y1="0" x2="1" y2="1">
    <stop offset="0" stop-color="#04050d"/><stop offset="0.5" stop-color="#0a1030"/>
    <stop offset="1" stop-color="#121a4d"/>
  </linearGradient>
  <radialGradient id="halo" cx="74%" cy="46%" r="42%">
    <stop offset="0" stop-color="#22d3ff" stop-opacity="0.42"/>
    <stop offset="0.55" stop-color="#3355ff" stop-opacity="0.14"/>
    <stop offset="1" stop-color="#3355ff" stop-opacity="0"/>
  </radialGradient>
  <filter id="glow" x="-40%" y="-40%" width="180%" height="180%">
    <feGaussianBlur stdDeviation="3.2" result="b"/>
    <feMerge><feMergeNode in="b"/><feMergeNode in="SourceGraphic"/></feMerge>
  </filter>
  <pattern id="dots" width="26" height="26" patternUnits="userSpaceOnUse">
    <circle cx="2" cy="2" r="1" fill="#7fdfff" opacity="0.07"/>
  </pattern>
  <style>
    .body{stroke:#38e0ff;stroke-width:2.4;fill:none;stroke-linecap:round;stroke-linejoin:round;}
    .key{stroke:#5fe6ff;stroke-width:1.5;fill:none;stroke-linecap:round;stroke-linejoin:round;}
    .mag{stroke:#ff5ce1;stroke-width:1.8;fill:none;stroke-linecap:round;stroke-linejoin:round;}
    .grn{stroke:#4dffa6;stroke-width:1.8;fill:none;stroke-linecap:round;stroke-linejoin:round;}
    .floor{stroke:#2f7bd6;stroke-width:1;fill:none;opacity:0.16;}
    .cable{stroke:#38e0ff;stroke-width:2.2;fill:none;stroke-linecap:round;}
    .node{fill:#bff4ff;}
    .dim{stroke:#8fd0ff;stroke-width:1.2;fill:none;opacity:0.6;stroke-dasharray:5 4;}
    .dimt{fill:#a9ecff;font-family:ui-monospace,Menlo,Consolas,monospace;font-size:20px;font-weight:600;letter-spacing:2.5px;opacity:0.95;}
    .h1{fill:#f3f7ff;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Helvetica,Arial,sans-serif;font-weight:800;font-size:52px;letter-spacing:-1.6px;}
    .sub{fill:#cfe6ff;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Helvetica,Arial,sans-serif;font-weight:400;font-size:22px;}
    .tag{fill:#5fe6ff;font-family:ui-monospace,Menlo,Consolas,monospace;font-size:14px;letter-spacing:2px;}
    .pill{fill:#7fe0ff;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif;font-weight:600;font-size:16px;}
  </style>
</defs>''')

out.append('<rect width="1280" height="480" rx="28" fill="url(#bg)"/>')
out.append('<rect width="1280" height="480" rx="28" fill="url(#dots)"/>')
out.append('<rect width="1280" height="480" rx="28" fill="url(#halo)"/>')

# suelo
out.append('<g>')
for (a,b) in floor:
    out.append(line(a,b,"floor"))
out.append('</g>')

# dispositivo (con glow)
out.append('<g filter="url(#glow)">')
# cable primero (detras)
cp = [place(p) for p in cable_pts]
out.append(f'<path class="cable" d="M{cp[0][0]:.1f} {cp[0][1]:.1f} C{cp[1][0]:.1f} {cp[1][1]:.1f} {cp[2][0]:.1f} {cp[2][1]:.1f} {cp[3][0]:.1f} {cp[3][1]:.1f}"/>')
for (a,b,kind) in segments:
    out.append(line(a,b,kind))
# nodos
for n in nodes:
    x,y = place(n)
    out.append(f'<circle cx="{x:.1f}" cy="{y:.1f}" r="2.3" class="node"/>')
out.append('</g>')

# etiqueta CAD en la columna izquierda (fuera del dibujo, para que no se pierda)
lx, ly = 70, 392
out.append(f'<line x1="{lx}" y1="{ly-17}" x2="{lx+46}" y2="{ly-17}" class="dim"/>')
out.append(f'<text x="{lx}" y="{ly}" class="dimt" filter="url(#glow)">MX KEYPAD  \u00b7  GRID 3 \u00d7 3</text>')

# ---- texto izquierda ----
out.append('<g>')
out.append('<rect x="46" y="112" width="7" height="150" rx="4" fill="#38e0ff"/>')
out.append('<text x="70" y="120" class="tag">// LOGI MX KEYPAD</text>')
out.append('<text x="68" y="176" class="h1">Microsoft Teams</text>')
out.append('<text x="68" y="234" class="h1">Controls</text>')
out.append('<text x="70" y="276" class="sub">Atajos de Teams para tu Logitech MX Keypad</text>')
# pills
out.append('<rect x="70" y="300" width="150" height="34" rx="17" fill="none" stroke="#38e0ff" stroke-opacity="0.5"/>')
out.append('<text x="145" y="323" class="pill" text-anchor="middle">22 acciones</text>')
out.append('<rect x="232" y="300" width="126" height="34" rx="17" fill="none" stroke="#38e0ff" stroke-opacity="0.5"/>')
out.append('<text x="295" y="323" class="pill" text-anchor="middle">4 grupos</text>')
out.append('</g>')

out.append('</svg>')

open('readme-header.svg','w').write('\n'.join(out))
print("wrote readme-header.svg")
