﻿using MusicStoreKeeper.Model;
using System.Collections.Generic;
using System.IO;

namespace Common
{
    public interface IImageService
    {
        int GetNumberOfImagesInCollection(IEnumerable<ImageData> imageDataList);

        IEnumerable<string> GetDuplicateImagePaths(IEnumerable<FileInfo> imageFileInfos, byte threshold = default, float imageDifferenceLimit = 0.1F);
    }
}