ldc i 2
dpl i
str i 0 0
l2:
lod i 0 0
conv i i
ldc i 100
conv i i
leq i
fjp l1
ldc i 1
dpl i
str i 0 1
l3:
lod i 0 1
conv i i
ldc i 1
conv i i
add i
dpl i
str i 0 1
lod i 0 0
conv i i
lod i 0 1
conv i i
equ i
fjp l5
lod i 0 0
out i
ujp l4
ujp l6
l5:
l6:
lod i 0 0
conv i i
lod i 0 1
conv i i
mod
conv i i
ldc i 0
conv i i
equ i
not
fjp l4
ujp l3
l4:
l0:
lod i 0 0
conv i i
ldc i 1
conv i i
add i
dpl i
str i 0 0
ujp l2
l1:
hlt
