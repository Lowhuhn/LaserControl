

extern "C"
{
	/*
	unsigned char* Image = NULL;
	int Width = 0;
	int Height = 0;
	int WidthHeight = 0;

	int CenterX = 0;
	int CenterY = 0;

	//0 = Eingabe, 1 = Inverted, 2 = Normalized, 3 = BlackWhite,
	int DrawingOutputImage = 0;
	//0 = kein Kreuz, 1 = Mittelkreuz, 2 = Gefundenes Kreuz, 3 = Beide Kreuze
	int DrawingCross = 0;

	int rou(double val)
	{
	return (int)floor(val + 0.5);
	}

	void ImageProcessing_Normalization(int *img){
	int* vals = (int*)malloc(256 * sizeof(int));
	int* cfd = (int*)malloc(256 * sizeof(int));
	int* h = (int*)malloc(256 * sizeof(int));
	for (int i = 0; i < 256; ++i){
	vals[i] = 0;
	}
	int wh = Width*Height;
	for (int i = 0; i < wh; ++i){
	vals[img[i]]++;
	}
	int minCFD = 0;
	for (int i = 0; i < 256; ++i){
	minCFD += vals[i];
	cfd[i] = minCFD;
	}
	int j = 0;
	while (vals[j] == 0)
	j++;
	minCFD = cfd[j];
	for (int i = 0; i < 256; ++i){
	if (vals > 0){
	h[i] = rou((((double)(cfd[i] - minCFD)) / ((double)(wh - minCFD)))*255.0);
	}
	}
	for (int i = 0; i < wh; ++i){
	img[i] = h[img[i]];
	}

	free(vals);
	free(cfd);
	free(h);
	}

	int cmpfunc(const void * a, const void * b)
	{
	return (*(int*)a - *(int*)b);
	}

	//2D Median Filter
	void ImageProcessing_NoiseReduction(int *img){
	int* use = (int*)calloc(Width*Height, sizeof(int));

	//Window Size = 9 x 9
	int winSize = 9;
	int n = (winSize*winSize) / 2;
	int edgeX = winSize / 2;
	int edgeY = winSize / 2;
	int* colorArray = (int*)malloc(winSize * winSize * sizeof(int));

	for (int x = edgeX; x < Width - edgeX; ++x){
	for (int y = edgeY; y < Height - edgeY; ++y){

	for (int fx = 0; fx < winSize; ++fx){
	for (int fy = 0; fy < winSize; ++fy){
	colorArray[fx * winSize + fy] = img[(x + fx - edgeX) + ((y + fy - edgeY)*Width)];
	}
	}
	qsort(colorArray, winSize*winSize, sizeof(int), cmpfunc);
	use[x + y*Width] = colorArray[n];
	}
	}


	for (int i = 0; i < Width*Height; ++i){
	img[i] = use[i];
	}
	}

	void ImageProcessing_BlackWhite(int* img){
	for (int i = 0; i < Width*Height; ++i){
	img[i] = img[i]>220?255:0;
	}
	}

	void ImageProcessing_BlackWhite2(int* img){
	int* vals = (int*)malloc(256 * sizeof(int));
	int wh = Width*Height;
	int mit = 0;
	int medI = 0;
	for (int i = 0; i < 256; ++i){
	vals[i] = 0;
	}
	for (int i = 0; i < wh; ++i){
	++vals[img[i]];
	mit += img[i];
	}
	mit /= wh;
	medI = wh / 2;
	for (int i = 0; i < 256; ++i){
	medI -= vals[i];
	if (medI <= 0)
	{
	medI = i;
	break;
	}
	}
	printf("Mit: %d\nMed: %d\n", mit, medI);

	free(vals);
	}

	void ImageProcessing_InvertIfNecessary(int* img){
	int wh = Width*Height;
	int black = 0;
	int white = 0;
	int i = 0;

	for (i = 0; i < wh; ++i){
	if (img[i] > 128)
	white++;
	else
	black++;
	}
	if (white > black){
	for (i = 0; i < wh; ++i)
	img[i] = 255 - img[i];

	}
	}

	void ImageProcessing_InvertIfNecessary2(int* img){
	int wh = Width*Height;
	int med = 0;
	int i = 0;

	for (i = 0; i < wh; ++i){
	med += img[i];
	}
	med /= wh;
	if (med > 128){
	for (i = 0; i < wh; ++i)
	img[i] = 255 - img[i];

	}
	}

	void ImageProcessing_InvertIfNecessary3(int* img){
	int wh = Width*Height;
	int med = 0;
	int i = 0;

	for (i = 0; i < wh; ++i){
	med += img[i];
	}
	med /= wh;
	if (med < 128){
	for (i = 0; i < wh; ++i)
	img[i] = 255 - img[i];

	}

	}

	void ImageProcessing_EdgeDetection_Sobel(int *img){
	int y = 0;
	int x = 0;
	int py = 0;
	int px = 0;
	int HM1 = Height - 1;
	int WM1 = Width - 1;
	int x_Val = 0, y_Val = 0;
	int wh = Width * Height;

	int* use = (int*)malloc(wh*sizeof(int));

	for (y = 1; y < HM1; ++y){
	py = Width*y;
	for (x = 1; x < WM1; ++x){
	px = py + x;

	//X_Sobel
	x_Val = 1*(img[px - 1 - Width] - img[px + 1 - Width]) +
	2*(img[px - 1]  - img[px + 1] ) +
	1*(img[px - 1 + Width] - img[px + 1 + Width]);
	y_Val = (1*img[px - 1 - Width] + 2*img[px - Width] + 1*img[px + 1 - Width]) -
	(1*img[px - 1 - Width] + 2*img[px - Width] + 1*img[px + 1 - Width]);
	//use[px] = abs(x_Val)+abs(y_Val);
	use[px] = sqrtl(x_Val*x_Val + y_Val*y_Val);
	}
	}

	for (x = 0; x < wh; ++x){
	if (use[x] > 255)
	img[x] = 255;
	else
	img[x] = use[x];
	}
	free(use);
	}

	void ImageProcessing_CrossCenterDetection(int *img){
	int x = 0, y = 0, py = 0;

	NUM a = 0, bc = 0, d = 0, e = 0, f = 0, m = 0, n = 0;
	NUM bcy = 0, dy = 0, ey = 0, fy = 0, my = 0, ny = 0;
	NUM cx = 0;

	for (y = 0; y < Height; ++y){
	py = y*Width;
	for (x = 0; x < Width; ++x){
	if (img[py + x] > 128){
	++a;

	//X
	bc += x;
	d += (x*x);
	e += y;
	f += (x*y);

	//Y
	dy += (y*y);
	}
	}
	}
	//Set values that are equal
	bcy = e;
	ey = bc;
	fy = f;

	f -= ((bc / a)*e);
	d -= ((bc / a)*bc);
	n = f / d;
	bc *= n;
	e -= bc;
	m = e / a;

	fy -= ((bcy / a)*ey);
	dy -= ((bcy / a)*bcy);
	ny = fy / dy;
	bcy *= ny;
	ey -= bcy;
	my = ey / a;


	cx = ((m*ny) + my) / (1 - n*ny);

	CenterX = (int)cx;
	CenterY = (int)(m + (n*cx));
	}

	void ImageProcessing_EdgeDetection_LeftRight(int *img){
	int x = 0, y = 0, py = 0;
	int HM1 = Height - 1;
	int WM1 = Width - 1;
	int wh = Width*Height;
	int* use = (int*)malloc(Width*Height*sizeof(int));
	for (y = 1; y < HM1; ++y){
	py = y*Width;
	for (x = 1; x < WM1; ++x){
	//use[py + x] = (img[py + x - 1] != img[py + x + 1]) ? 255 : 0;
	use[py + x] = (img[py + x - Width] != img[py + x + Width]) ? 255 : 0;
	}
	}
	for (x = 0; x < wh; ++x)
	img[x] = use[x];
	free(use);
	}

	void ImageProcessing_CrossCenterDetection_2(int *img){
	int x = 0, y = 0, py = 0;
	int wh = Width*Height;

	NUM n = 0;
	NUM a = 0, b = 0,c = 0,d = 0,e = 0,f = 0,g=0, sxx = 0,sx = 0,sxy = 0;
	NUM b1 = 0, b0 = 0;
	//Count all Elements
	for (x = 0; x < wh; ++x){
	if (img[x] > 128)
	++n;
	}
	int local_y = 0;
	for (y = 0; y < Height; ++y){
	py = y*Width;
	local_y = y - Height / 2;
	for (x = 0; x < Width; ++x){
	if (img[py + x] > 128){
	a += x;
	b += x*x;

	e += local_y;
	f += x*local_y;
	}
	}
	}
	c = a / n;
	d = b / n;
	g = e / n;
	sxx = b - d;
	sx = sqrt(sxx / (n - 1));

	sxy = f - ((a*e) / n);

	b1 = sxy / sxx;
	b0 = g - (b1*c);

	CenterY = (int)(b0 + (b1*(Width / 2))) + (Height / 2);
	}

	void ImageProcessing_ChangeBG(int *img){
	int wh = Width*Height;
	//int* use = (int*)malloc(wh*sizeof(int));
	int col_width = ceill(Width / 128.0);
	int col_height = ceill(Height / 128.0);

	int scolor = 0;
	int py = 0,x = 0, v = 0;
	for (int y = 0; y < Height; ++y)
	{
	py = y*Width;
	scolor = (y / col_height);
	for (x = 0; x < Width; ++x)
	{
	v = 255 - scolor - (x / col_width);
	if (v > 0){
	img[py + x] = (int)(255 * ((double)img[py + x] / (double)v));
	}
	}
	}
	/*for (x = 0; x < wh; ++x)
	img[x] = use[x];
	free(use);
	}

	__declspec(dllexport) void SetImagePointer(void *a, int width, int height)
	{
	Image = (unsigned char*)a;
	Width = width;
	Height = height;
	WidthHeight = Width*Height;
	}

	__declspec(dllexport) void ImageProcess_FindAndMarkCross()
	{
	int y = 0;
	int widthheight = Width*Height;

	uchar* img = Image;
	int* use = (int*)malloc(widthheight*sizeof(int));
	//Convert to Grayscale
	for (int i = 0; i < widthheight; ++i){
	y = (int)(img[0] * 0.3 + img[1] * 0.59 + img[2] * 0.11);
	img[0] = img[1] = img[2] = y;
	use[i] = y > 255 ? 255 : y;
	img += 3;
	}

	//ImageProcessing_InvertIfNecessary3(use);
	ImageProcessing_Normalization(use);
	ImageProcessing_ChangeBG(use);
	//ImageProcessing_CrossCenterDetection(use);
	if (DrawingOutputImage == 2){
	img = Image;
	for (int i = 0; i < widthheight; ++i){
	*(img) = img[1] = img[2] = (uchar)use[i];
	img += 3;
	}
	}
	/*ImageProcessing_InvertIfNecessary2(use);
	if (DrawingOutputImage == 1){
	img = Image;
	for (int i = 0; i < widthheight; ++i){
	*(img) = img[1] = img[2] = (uchar)use[i];
	img += 3;
	}
	}*/

	/*ImageProcessing_Normalization(use);
	ImageProcessing_Normalization(use);
	if (DrawingOutputImage == 2){
	img = Image;
	for (int i = 0; i < widthheight; ++i){
	*(img) = img[1] = img[2] = (uchar)use[i];
	img += 3;
	}
	}
	/*
	ImageProcessing_InvertIfNecessary3(use);
	if (DrawingOutputImage == 3){
	img = Image;
	for (int i = 0; i < widthheight; ++i){
	*(img) = img[1] = img[2] = (uchar)use[i];
	img += 3;
	}
	}

	ImageProcessing_BlackWhite2(use);
	if (DrawingOutputImage == 4){
	img = Image;
	for (int i = 0; i < widthheight; ++i){
	*(img) = img[1] = img[2] = (uchar)use[i];
	img += 3;
	}
	}
	//ImageProcessing_CrossCenterDetection(use);

	//2. RUN :
	/*
	for (int i = 2; i <= 8; ++i){
	int w2 = Width / i;
	int w4 = (Width / i) / 2;
	int nx0 = CenterX - w4;
	nx0 = nx0 < 0 ? 0 : nx0; // Kleiner 0?
	nx0 = (nx0 + w2) >= Width ? Width - 1 - w2 : nx0; // Zu nah an width rand
	int nx1 = nx0 + w2;

	int h2 = Height / i;
	int h4 = (Height / i) / 2;
	int ny0 = CenterY - h4;
	ny0 = ny0 < 0 ? 0 : ny0;
	ny0 = (ny0 + h2) >= Height ? Height - 1 - h2 : ny0;
	int ny1 = ny0 + h2;

	int py = 0, x = 0;
	for (int y = 0; y < Height; ++y){
	py = y*Width;
	for (x = 0; x < Width; ++x){
	if (x < nx0 || x > nx1 || y < ny0 || y > ny1)
	use[py + x] = 0;
	}
	}
	ImageProcessing_CrossCenterDetection(use);
	}*/
	//Write Back Data to Image
	/*img = Image;
	for (int i = 0; i < widthheight; ++i){
	*(img) = img[1] = img[2] = (uchar)use[i];
	img+=3;
	}

	if (DrawingCross == 1 || DrawingCross == 3){
	int cx = Width / 2;
	int cy = Height / 2;
	img = &Image[cy*Width * 3];
	for (int i = 0; i < Width; ++i){
	img[0] = 0;
	img[1] = 0;//Green
	img[2] = 255;//Red
	img += 3;
	}
	img = Image;
	int p = 0;
	for (int i = 0; i < Height; ++i){
	p = (i*Width) * 3 + cx * 3;
	img[p] = 0;
	img[p + 1] = 0;//Green
	img[p + 2] = 255;//Red
	}
	}

	if (DrawingCross == 2 || DrawingCross == 3){
	img = &Image[CenterY*Width * 3];
	for (int i = 0; i < Width; ++i){
	img[0] = 0;
	img[1] = 255;//Green
	img[2] = 0;//Red
	img += 3;
	}
	img = Image;
	int p = 0;
	for (int i = 0; i < Height; ++i){
	p = (i*Width) * 3 + CenterX * 3;
	img[p] = 0;
	img[p + 1] = 255;//Green
	img[p + 2] = 0;//Red
	}
	}
	free(use);
	}

	__declspec(dllexport) void ImageProcess_FindAndMarkCross_2()
	{
	int y = 0;
	int widthheight = Width*Height;

	uchar* img = Image;
	int* use = (int*)malloc(widthheight*sizeof(int));
	//Convert to Grayscale
	for (int i = 0; i < widthheight; ++i){
	y = (int)(img[0] * 0.3 + img[1] * 0.59 + img[2] * 0.11);
	img[0] = img[1] = img[2] = y;
	use[i] = y > 255 ? 255 : y;
	img += 3;
	}

	/*ImageProcessing_InvertIfNecessary2(use);
	if (DrawingOutputImage == 1){
	img = Image;
	for (int i = 0; i < widthheight; ++i){
	*(img) = img[1] = img[2] = (uchar)use[i];
	img += 3;
	}
	}

	ImageProcessing_Normalization(use);
	if (DrawingOutputImage == 2){
	img = Image;
	for (int i = 0; i < widthheight; ++i){
	*(img) = img[1] = img[2] = (uchar)use[i];
	img += 3;
	}
	}

	ImageProcessing_InvertIfNecessary3(use);
	if (DrawingOutputImage == 3){
	img = Image;
	for (int i = 0; i < widthheight; ++i){
	*(img) = img[1] = img[2] = (uchar)use[i];
	img += 3;
	}
	}

	ImageProcessing_BlackWhite(use);
	if (DrawingOutputImage == 4){
	img = Image;
	for (int i = 0; i < widthheight; ++i){
	*(img) = img[1] = img[2] = (uchar)use[i];
	img += 3;
	}
	}
	ImageProcessing_CrossCenterDetection(use);

	//2. RUN :

	for (int i = 2; i <= 8; ++i){
	int w2 = Width / i;
	int w4 = (Width / i) / 2;
	int nx0 = CenterX - w4;
	nx0 = nx0 < 0 ? 0 : nx0; // Kleiner 0?
	nx0 = (nx0 + w2) >= Width ? Width - 1 - w2 : nx0; // Zu nah an width rand
	int nx1 = nx0 + w2;

	int h2 = Height / i;
	int h4 = (Height / i) / 2;
	int ny0 = CenterY - h4;
	ny0 = ny0 < 0 ? 0 : ny0;
	ny0 = (ny0 + h2) >= Height ? Height - 1 - h2 : ny0;
	int ny1 = ny0 + h2;

	int py = 0, x = 0;
	for (int y = 0; y < Height; ++y){
	py = y*Width;
	for (x = 0; x < Width; ++x){
	if (x < nx0 || x > nx1 || y < ny0 || y > ny1)
	use[py + x] = 0;
	}
	}
	ImageProcessing_CrossCenterDetection(use);
	}

	if (DrawingCross == 1 || DrawingCross == 3){
	int cx = Width / 2;
	int cy = Height / 2;
	img = &Image[cy*Width * 3];
	for (int i = 0; i < Width; ++i){
	img[0] = 0;
	img[1] = 0;//Green
	img[2] = 255;//Red
	img += 3;
	}
	img = Image;
	int p = 0;
	for (int i = 0; i < Height; ++i){
	p = (i*Width) * 3 + cx * 3;
	img[p] = 0;
	img[p + 1] = 0;//Green
	img[p + 2] = 255;//Red
	}
	}

	if (DrawingCross == 2 || DrawingCross == 3){
	img = &Image[CenterY*Width * 3];
	for (int i = 0; i < Width; ++i){
	img[0] = 0;
	img[1] = 255;//Green
	img[2] = 0;//Red
	img += 3;
	}
	img = Image;
	int p = 0;
	for (int i = 0; i < Height; ++i){
	p = (i*Width) * 3 + CenterX * 3;
	img[p] = 0;
	img[p + 1] = 255;//Green
	img[p + 2] = 0;//Red
	}
	}
	free(use);
	}

	__declspec(dllexport) void SetParameter(int outputImage, int outputCross){
	if (outputImage < 0)
	outputImage *= (-1);
	if (outputCross < 0)
	outputCross *= (-1);

	DrawingOutputImage = outputImage;
	DrawingCross = outputCross;

	}

	__declspec(dllexport) void TestAndLoadLib(){
	printf("Use C Lib for cross detection!\n");
	}

	__declspec(dllexport) void ImageProcess_Find(){

	int* use = (int*)malloc(WidthHeight + sizeof(int));

	}
	}




	/*
	for (int i = 1; i < heightM1; ++i){
	py = i*Width;
	for (j = 0; j < widthM1; ++j){
	pyj = py + j;
	gx = (useB[pyj - 1 - Width]*3 - useB[pyj + 1 - Width]*3)+
	(useB[pyj - 1]*10 - useB[pyj + 1]*10)+
	(useB[pyj -1 +Width]*3 - useB[pyj+1+Width]*3);
	gy = (useB[pyj - 1 - Width]*3 - useB[pyj - 1 + Width]*3) +
	(useB[pyj - Width]*10 - useB[pyj + Width]*10) +
	(useB[pyj + 1 - Width]*3 - useB[pyj + 1 + Width]*3);
	if (gx != 0 || gy != 0){
	use[pyj] = 255;
	}
	else{
	use[pyj] = 0;
	}
	}
	}
	*/

	/*for (int i = 1; i < heightM1; ++i){
	py = i*Width;
	for (j = 0; j < widthM1; ++j){
	pyj = py + j;
	y = (use[pyj - 1 - Width] + use[pyj - Width] * 2 + use[pyj + 1 - Width]) +
	(use[pyj - 1] * 2 + use[pyj] * 4 + use[pyj + 1] * 2) +
	(use[pyj - 1 + Width] + use[pyj + Width] * 2 + use[pyj + 1 + Width]);
	useB[pyj] = (y/16);
	}
	}*/

	/*for (int i = 2; i < Height - 2; ++i){
	py = i*Width;
	for (int j = 2; j < Width - 2; ++j){
	pyj = py + j;
	y = (2 * use[pyj - 2 - 2 * Width]) +
	(4 * use[pyj - 1 - 2 * Width]) +
	(5 * use[pyj - 2 * Width]) +
	(4 * use[pyj + 1 - 2 * Width]) +
	(2 * use[pyj + 2 - 2 * Width]) +
	//Zeile 2
	(4 * use[pyj - 2 - Width]) +
	(9 * use[pyj - 1 - Width]) +
	(12 * use[pyj - Width]) +
	(9 * use[pyj + 1 - Width]) +
	(4 * use[pyj + 2 - Width]) +
	//Zeile 3
	(5 * use[pyj - 2]) +
	(12 * use[pyj - 1]) +
	(15 * use[pyj]) +
	(12 * use[pyj + 1]) +
	(5 * use[pyj + 2]) +
	//Zeile 4
	(4 * use[pyj - 2 + Width]) +
	(9 * use[pyj - 1 + Width]) +
	(12 * use[pyj + Width]) +
	(9 * use[pyj + 1 + Width]) +
	(4 * use[pyj + 2 + Width]) +
	//Zeile 5
	(2 * use[pyj - 2 + 2 * Width]) +
	(4 * use[pyj - 1 + 2 * Width]) +
	(5 * use[pyj + 2 * Width]) +
	(4 * use[pyj + 1 + 2 * Width]) +
	(2 * use[pyj + 2 + 2 * Width]);
	useB[pyj] = (y/159);
	}
	}*/
}