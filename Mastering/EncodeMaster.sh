ffmpeg.exe -i MasterCF.mov -c:v prores_ks -pix_fmt yuv422p10le -profile:v 3 -qscale:v 9 -acodec copy Master.mov
