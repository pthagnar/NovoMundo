from PIL import Image

img = Image.open('worldmap.bmp').convert('L')  # convert image to 8-bit grayscale
WIDTH, HEIGHT = img.size

data = list(img.getdata()) # convert image data to a list of integers
# convert that to 2D list (list of lists of integers)
data = [data[offset:offset+WIDTH] for offset in range(0, WIDTH*HEIGHT, WIDTH)]

a = [];
for line in data:
   l = [];
   for item in line:
      l = l + [item]
   a = a + [l]
 
for line in a:
   print " ".join([str(x) for x in line])

