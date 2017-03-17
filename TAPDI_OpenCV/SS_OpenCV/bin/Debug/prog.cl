

__kernel void multiply(__global float* arr,__global long* aux1)
{
    int i = get_global_id(0);
    arr[i] = arr[i] * arr[i];


}

__kernel void negative_uncoalesced(__global uchar* image, int w, int h, int padding, __global uchar* imageOut)
{
    int i = get_global_id(0)+get_global_id(1)*get_global_size(0);
    int idx = i*3 + (i/w) * padding;

    if (i < w*h) {
		imageOut[idx] = 255 - image[idx];
		imageOut[idx+1] = 255 - image[idx+1];
		imageOut[idx+2] = 255 - image[idx+2];
    }

}

__kernel void negative_uncoalesced_xy(__global uchar* image, int w, int h, int padding, __global uchar* imageOut)
{
    int x = get_global_id(0);
	int y = get_global_id(1);
    int idx = y * (w*3 + padding) + x*3 ;

    if ((x < w) && (y < h)) {
		imageOut[idx] = 255 - image[idx];
		imageOut[idx+1] = 255 - image[idx+1];
		imageOut[idx+2] = 255 - image[idx+2];
    }

}

__kernel void negative_uint4(__global uchar* image, int w, int h,  int padding, __global uchar4* imageOut)
{
    int i = get_global_id(0)+get_global_id(1)*get_global_size(0);
    int idx = i + (i/w) * padding/4 ;
    uchar4 neg = (uchar4)(255,255,255,0);

    if (i < w*h) {
		imageOut[idx] = neg - image[idx];
    }
}

__constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE | //Natural coordinates
							CLK_ADDRESS_CLAMP_TO_EDGE | //Clamp to zeros
							CLK_FILTER_NEAREST;

__kernel void negative_image2D(__read_only image2d_t image, __write_only image2d_t imageOut,  int w, int h)
{
    int iX = get_global_id(0);
    int iY = get_global_id(1);
    uint4 neg = (uint4)(255, 255, 255 , 255);

    if ((iX >= 0)&&(iX < w) && (iY >= 0)&&(iY < h)) {
		uint4 pixelV = read_imageui( image, sampler, (int2)(iX,iY));
		write_imageui( imageOut, (int2)(iX,iY) , neg - pixelV );
    }
}



__kernel void template_match_image2d(__read_only image2d_t image, __read_only image2d_t imageTemplate, __write_only image2d_t imageOut, 
 int w, int h,  int w_t, int h_t)
{	
    int iX = get_global_id(0);
    int iY = get_global_id(1);
	uint4 pixelV, pixelT, aux;
	float4 pixelOut,auxf;
	int t_y, t_x;

	int length_x = w - w_t +1;
	int length_y = h - h_t +1; 

    if ((iX >= 0) && (iX < length_x) && (iY >= 0) && (iY < length_y)) {

		pixelOut = (float4) (0,0,0,0);
		
		//pixelT = (uint4) (0,0,0,0);

		for( t_y=0; t_y< length_y; t_y++){
		
			for(t_x=0; t_x < length_x; t_x++){

				pixelV = read_imageui( image, sampler, (int2)(iX+t_x , iY+t_y));
				pixelT = read_imageui( imageTemplate, sampler, (int2)(t_x , t_y));
/*	
				pixelOut.x += pow(pixelV.x - pixelT.x , 2.0f);
				pixelOut.y +=  pow(pixelV.y - pixelT.y,2.0f);
				pixelOut.z +=  pow(pixelV.z - pixelT.z,2.0f);
				pixelOut.w +=  pow(pixelV.w - pixelT.w,2.0f);
				*/
				//aux =  abs_diff((int4)pixelV , (int4)pixelT);

			//	auxf = (float4)( pixelV - pixelT);
				//pixelOut += auxf*auxf;
			}
		}

		write_imagef( imageOut, (int2)(iX,iY) , pixelOut );

/*
pixelV = read_imageui( image, sampler, (int2)(iX , iY));
				
		write_imageui( imageOut, (int2)(iX,iY) , pixelV );*/
    }
}




__kernel void template_match_(__global uchar4* image, __global uchar4* imageTemplate, __global float* imageOut, 
 int w, int h, int padding , int w_t, int h_t, int padding_t)
{	
	



    int iX = get_global_id(0);
    int iY = get_global_id(1);

	uchar4  pixelV, pixelT, aux;

	float  pixelOut;
	int    t_y, t_x;

	int length_x =  w - w_t +1;
	int length_y =  h - h_t +1; 

	int idxf = iX + iY*(length_x);
	int idx, idx_t;
	int dif_x, dif_y, dif_z;

    if ((iX >= 0) && (iX < length_x) && (iY >= 0) && (iY < length_y)) {

		pixelOut = 0;
		
		for( t_y=0; t_y< 22; t_y++){
		
			for(t_x=0; t_x < 22; t_x++){
				idx = (iX+t_x) + (iY+t_y)*(w + padding/4);
				idx_t = t_x + t_y *(w_t + padding_t/4);

				pixelV = image[idx];
				pixelT =  imageTemplate[idx_t];
				
				dif_x = pixelV.x -pixelT.x;
				dif_y = pixelV.y -pixelT.y;
				dif_z = pixelV.z -pixelT.z;

				pixelOut += dif_x * dif_x + dif_y * dif_y + dif_z * dif_z;
			}
		}
		
		imageOut[idxf] = pixelOut ;
    }
}

__kernel void template_match(__global uchar4* image, __global uchar4* imageTemplate, __global float* imageOut, 
 int w, int h, int padding , int w_t, int h_t, int padding_t)
{	
	__local uchar4 local_template_image[22*22 * 2]; //484 + padding
	__local uchar4 local_image[22*22 * 4]; //484 + padding

	int indexL = get_local_id(0) + get_local_id(1)*get_local_size(1);
    int iX = get_local_id(0);
    int iY = get_local_id(1);

	if (iX < 22 && iY < 22)
		local_template_image[indexL] = imageTemplate[iX + iY *(w_t + padding_t/4)];

	barrier(CLK_GLOBAL_MEM_FENCE);


	iX = get_global_id(0);
	iY = get_global_id(1);

	uchar4  pixelV, pixelT;
	float  pixelOut;
	int    t_y, t_x;

	int length_x =  w - w_t +1;
	int length_y =  h - h_t +1; 

	int idx;
	int dif_x, dif_y, dif_z;

    if ((iX >= 0) && (iX < length_x) && (iY >= 0) && (iY < length_y)) {

		pixelOut = 0;
		
		for( t_y=0; t_y< 22; t_y++){
		
			for(t_x=0; t_x < 22; t_x++){
				idx = (iX+t_x) + (iY+t_y)*(w + padding/4);
				pixelV = image[idx];
				
				idx = t_x + t_y *22;
				pixelT =  local_template_image[idx];
				
				dif_x = pixelV.x -pixelT.x;
				dif_y = pixelV.y -pixelT.y;
				dif_z = pixelV.z -pixelT.z;

				pixelOut += dif_x * dif_x + dif_y * dif_y + dif_z * dif_z;
			}
		}
		
		imageOut[iX + iY * (length_x)] = pixelOut ;
    }
}

/*

__kernel void mean_value(__global uchar4* image, __global uint* outvalues, 
 int w, int h, int padding )
{	
	__local uint local_mean[3]; //484 + padding


	int indexL = get_local_id(0) + get_local_id(1)*get_local_size(1);
    int iX = get_global_id(0);
    int iY = get_global_id(1);

	uchar4  pixelV;
	uint blue, red, green;
	if (iX < w && iY < h){
		pixelV = image[iX + iY *(w + padding/4)];
		blue = pixelV.x;
		green = pixelV.y;
		red = pixelV.z;

		atomic_add(&local_mean[0],  blue);
		atomic_add(&local_mean[1],  green);
		atomic_add(&local_mean[2],  red);
	
		// the first thread copies the workgroup value to the global memory
		if (get_local_id(0) == 0 && get_local_id(1))
		{

			barrier(CLK_LOCAL_MEM_FENCE);

			atomic_add(&outvalues[0], local_mean[0]);
			atomic_add(&outvalues[1], local_mean[1]);
			atomic_add(&outvalues[2], local_mean[2]);
		}
	}
}
*/


__kernel void mean_value(__global uchar4* image, __global uint* outvalues, 
 int w, int h, int padding )
{	

    int iX = get_global_id(0);
    int iY = get_global_id(1);

	uchar4  pixelV;
	uint blue, green, red;

	if (iX < w && iY < h){
		pixelV = image[iX + iY *(w + padding/4)];
		blue = pixelV.x;
		green = pixelV.y;
		red = pixelV.z;
		
		atomic_add(&outvalues[0], blue);
		atomic_add(&outvalues[1], green);
		atomic_add(&outvalues[2], red);
		
	}
}


__kernel void max_value(__global int* image, 
 int w, int h, int padding, 
__global int* maxvalues, 
__global uint* maxPos, 
__global int* minvalues, 
__global uint* minPos )
{	
    int iX = get_global_id(0);
    int iY = get_global_id(1);

	int  pixelV;
	

	if (iX < w && iY < h){
		pixelV = image[iX + iY *(w + padding/4)];


		if (atomic_max(maxvalues, pixelV) < pixelV)
		{			
			maxPos[0] = iX;
			maxPos[1] = iY;
		}
		if (atomic_min(minvalues, pixelV) > pixelV)
		{			
			minPos[0] = iX;
			minPos[1] = iY;
		}

		
	}
}





/*
__kernel void mean_sep(__global uchar4* image, __global uchar4* outvalues, 
 int w, int h, int padding )
{	
	__local uint local_mean[3]; //484 + padding


	int indexL = get_local_id(0) + get_local_id(1)*get_local_size(1);
    int iX = get_global_id(0);
    int iY = get_global_id(1);

	uchar4  pixelV;
	uint blue, red, green;
	if (iX < w && iY < h){
		pixelV = image[iX + iY *(w + padding/4)];
		blue = pixelV.x;
		green = pixelV.y;
		red = pixelV.z;



		atomic_add(&local_mean[0],  blue);
		atomic_add(&local_mean[1],  green);
		atomic_add(&local_mean[2],  red);
	
		// the first thread copies the workgroup value to the global memory
		if (get_local_id(0) == 0 && get_local_id(1) == 0)
		{

			barrier(CLK_LOCAL_MEM_FENCE);

			atomic_add(&(outvalues[iX + iY *(w + padding/4)].x), local_mean[0]);
			atomic_add(&(outvalues[iX + iY *(w + padding/4)].y), local_mean[1]);
			atomic_add(&(outvalues[iX + iY *(w + padding/4)].z), local_mean[2]);
		}
	}
}


*/



 __kernel void connect_components_DOWN(__read_only image2d_t image, __write_only image2d_t imageOut, __global int* flag)
{	
    int iX = get_global_id(0);
    int iY = get_global_id(1);
	int imageW = get_image_width(image);
	int imageH = get_image_height(image);

	uint4 pixelV1,pixelV2,pixelV3,pixelV4,pixelV5, pixelOut;

    if ((iX >= 0) && (iX < imageW) && (iY >= 0) && (iY < imageH)) {

		pixelOut = (uint4) (0,0,0,0);
		
		pixelV1 = read_imageui( image, sampler, (int2)(iX-1 , iY-1));
		pixelV2 = read_imageui( image, sampler, (int2)(iX   , iY-1));
		pixelV3 = read_imageui( image, sampler, (int2)(iX+1 , iY-1));
		pixelV4 = read_imageui( image, sampler, (int2)(iX-1 , iY));
		pixelV5 = read_imageui( image, sampler, (int2)(iX   , iY));

		if (pixelOut.x > pixelV1.x) // min
			pixelOut = pixelV1;
		if (pixelOut.x > pixelV2.x) // min
			pixelOut = pixelV2;
		if (pixelOut.x > pixelV3.x) // min
			pixelOut = pixelV3;
		if (pixelOut.x > pixelV4.x) // min
			pixelOut = pixelV4;
		if (pixelOut.x > pixelV5.x) // min
			pixelOut = pixelV5;
	
		write_imageui( imageOut, (int2)(iX,iY) , pixelOut );

		*flag = 1;
    }
}

 __kernel void connect_components_UP(__read_only image2d_t image, __write_only image2d_t imageOut, __global int* flag)
{	
    int iX = get_global_id(0);
    int iY = get_global_id(1);
	int imageW = get_image_width(image);
	int imageH = get_image_height(image);

	uint4 pixelV1,pixelV2,pixelV3,pixelV4,pixelV5, pixelOut;

    if ((iX >= 0) && (iX < imageW) && (iY >= 0) && (iY < imageH)) {

		pixelOut = (uint4) (0,0,0,0);
		
		pixelV1 = read_imageui( image, sampler, (int2)(iX-1 , iY+1));
		pixelV2 = read_imageui( image, sampler, (int2)(iX   , iY+1));
		pixelV3 = read_imageui( image, sampler, (int2)(iX+1 , iY+1));
		pixelV4 = read_imageui( image, sampler, (int2)(iX+1 , iY));
		pixelV5 = read_imageui( image, sampler, (int2)(iX   , iY));

		if (pixelOut.x > pixelV1.x) // min
			pixelOut = pixelV1;
		if (pixelOut.x > pixelV2.x) // min
			pixelOut = pixelV2;
		if (pixelOut.x > pixelV3.x) // min
			pixelOut = pixelV3;
		if (pixelOut.x > pixelV4.x) // min
			pixelOut = pixelV4;
		if (pixelOut.x > pixelV5.x) // min
			pixelOut = pixelV5;
	
		write_imageui( imageOut, (int2)(iX,iY) , pixelOut );

		*flag = 1;
    }
}


// PERFORMS HORIZONTAL AND VERTICAL PROJECTION
 __kernel void Proj_Hor_Ver_1(__read_only image2d_t image, __global int* projH, __global int* projV)
{	
    int iX = get_global_id(0);
    int iY = get_global_id(1);
	int imageW = get_image_width(image);
	int imageH = get_image_height(image);


	uint4 pixelV1;

    if ((iX < imageW) && (iY < imageH)) {

		
		pixelV1 = read_imageui( image, sampler, (int2)(iX , iY));

		if (max(max(pixelV1.x,pixelV1.y),pixelV1.z) > 0)
		{
			atomic_inc(&projH[iX]);
			atomic_inc(&projV[iY]);
		}
    }
}

// PERFORMS HORIZONTAL AND VERTICAL PROJECTION
 __kernel void Proj_Hor_Ver(__read_only image2d_t image, __global int* projH, __global int* projV)
{	
    int iX = get_global_id(0);
    int iY = get_global_id(1);

    int liX = get_local_id(0);
    int liY = get_local_id(1);

	int imageW = get_image_width(image);
	int imageH = get_image_height(image);
__local int localprojV[64];
__local int localprojH[64];

	uint4 pixelV1;

    if ((iX < imageW) && (iY < imageH)) {

		
		pixelV1 = read_imageui( image, sampler, (int2)(iX , iY));

		if (max(max(pixelV1.x,pixelV1.y),pixelV1.z) > 0)
		{
			atomic_inc(&localprojH[liX]);
			atomic_inc(&localprojV[liY]);
		}

    }
	barrier(CLK_LOCAL_MEM_FENCE);
	
	if (liY==0)
		atomic_add(&projH[iX], localprojH[liX]);

	if (liX==0)
		atomic_add(&projV[iY], localprojV[liY]);
	
}