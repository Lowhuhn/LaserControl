
#ifndef IMGPROCLIB_H
#define IMGPROCLIB_H

#include <ios>
#include <stdio.h>
#include <stdlib.h>

#include "ImgProcMath.h"

//Definitions
#define uchar unsigned char

//Methods

void crossCenterDetection(int *img, int width, int height, int threshold, int& centerX, int& centerY);
void crossCenterDetection(int *img, int width, int height, int threshold, int startX, int endX, int startY, int endY, int& centerX, int& centerY);

void crossCenterDetection_CoM(int *img, int width, int height, int threshold, int startX, int endX, int startY, int endY, int& centerX, int& centerY);

int* crossDetectionOfThreshold(int *img, int width, int height, int& centerX, int& centerY);

double* im2double(int* img, int whsize);

void imadjst(int* img, int whsize, double limit);
void imadjst(int* img, int whsize, int low, int high);

int* imextract(int* img, int width, int height, int x, int y, int ex_width, int ex_height);

void medianFilter2D(int* img, int width, int height, int filterSize);

void reduceBackground(int* img, int width, int height, int steps);

void reduceNoise(int* img, int width, int height, int steps);

//strechlim method to cancel out the lower and upper x percent of the pixels
void strechlim(int* img, int whsize, double limit, int& outlow, int& outhigh);

int* thresholdImage(int* img, int width, int height, int threshold);

int* edgeDetectionOfThreshold(int* img, int width, int height, int threshold, int which = 0);
int* edgeDetection(int* img, int width, int height, int threshold, int which);

#endif /* IMGPROCLIB_H */

