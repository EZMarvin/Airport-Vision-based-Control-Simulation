function [ dis ] = getdistance( r, g, b, height, width, background )
% input R G B matrix and dimension of picture and background data
% output two object distances in the input picture
% 1: reshape the RGB thrid matrixs into picture
r = reshape(r, width, height);
g = reshape(g, width, height);
b = reshape(b, width, height);
r = rot90(uint8(r));
g = rot90(uint8(g));
b = rot90(uint8(b));
picture(:, :, 1) = r;
picture(:, :, 2) = g;
picture(:, :, 3) = b;
% 2. change to gray picture and extract objects in the picture
safedistance = 400;
rgbimage = picture;
backimage = uint8(background);
backimage = reshape(backimage, 600, 1280, 3);
subplot(311);
imshow(rgbimage);
front = rgb2gray(rgbimage);
back = rgb2gray(backimage);
double = im2double(back - front);
objects = im2bw(double, 0.16);
se = strel('square', 10);
objects = imclose(objects, se);
objects = bwareaopen(objects, 1000);
objects = imfill(objects, 'holes');
subplot(312);
imshow(objects);
% 3. get boundaries of objects
boundaries = bwboundaries(objects);
numberOfboundaries = size(boundaries, 1);
% if there is only one object just return safe distance
if numberOfboundaries < 2
    dis = safedistance;
    return;
end
subplot(313);
imshow(objects);
hold on;
for k = 1 : numberOfboundaries
    thisBoundary = boundaries{k};
    plot(thisBoundary(:, 2), thisBoundary(:, 1), 'r', 'LineWidth', 3);
end
hold off;b1 = boundaries{1};
% calculate distance based on the boundaries of objects (closest two object)
b2 = boundaries{2};
dis = min(b2(:, 2)) - max(b1(:, 2));

end

