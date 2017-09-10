ffmpeg.exe -i Master_Main_CF.mov -c:v prores_ks -pix_fmt yuv422p10le -profile:v 3 -qscale:v 9 -acodec copy Master_Main.mov
ffmpeg.exe -i Master_Mask_CF.mov -c:v prores_ks -pix_fmt yuv422p10le -profile:v 3 -qscale:v 9 -acodec copy Master_Mask.mov
