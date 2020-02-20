using Common;
using System;
using System.Linq;

namespace FileAnalyzer
{
    public class MusicDirAnalyzer : IMusicDirAnalyzer
    {
        private readonly IMusicFileAnalyzer _musicFileAnalyzer;

        public MusicDirAnalyzer(IMusicFileAnalyzer musicFileAnalyzer)
        {
            _musicFileAnalyzer = musicFileAnalyzer;
        }

        #region [  public methods  ]

        public IMusicDirInfo AnalyzeMusicDirectory(ISimpleFileInfo dirSFi)
        {
            var mdi = new MusicDirInfo();
            mdi.Path = dirSFi.Path;
            //сортировка файлов и каталогов по типу
            foreach (var sfi in dirSFi.Children)
            {
                switch (sfi.Type)
                {
                    case SfiType.Directory:
                        //проверка на каталог с изображениями
                        if (CheckIfOnlyImageSubDirectory(sfi))
                        {
                            mdi.ImageDirectories.Add(sfi);
                            continue;
                        }
                        //проверка на вложенный альбом
                        if (CheckIfAudioFileSubDirectory(sfi))
                        {
                            mdi.SubDirectories.Add(sfi);
                            continue;
                        }
                        //проверка на вложенные каталоги
                        if (CheckIfHasSubdirectories(sfi))
                        {
                            mdi.SubDirectories.Add(sfi);
                        }
                        mdi.SubDirectories.Add(sfi);
                        break;

                    case SfiType.AudioFile:
                        mdi.TrackList.Add(sfi);
                        var trackInfo = _musicFileAnalyzer.GetBasicAlbumInfoFromAudioFile(sfi.Info);
                        mdi.TrackInfos.Add(trackInfo);
                        break;

                    case SfiType.ImageFile:
                        mdi.ImageFiles.Add(sfi);
                        break;

                    case SfiType.TextFile:
                        mdi.TextFiles.Add(sfi);
                        break;

                    case SfiType.Unknown:
                        mdi.UnknownFiles.Add(sfi);
                        break;

                    default:
                        throw new ArgumentException();
                }
            }

            return mdi;
        }

        #endregion [  public methods  ]

        #region [  private methods  ]

        //TODO:Add more checks
        private bool CheckIfHasSubdirectories(ISimpleFileInfo dirSfi)
        {
            return dirSfi.Children.Any(fi => fi.Type == SfiType.Directory);
        }

        private bool CheckIfOnlyImageSubDirectory(ISimpleFileInfo dirSfi)
        {
            return dirSfi.Children.All(fi => fi.Type == SfiType.ImageFile) & (dirSfi.Children.Count <= 10);
        }

        private bool CheckIfAudioFileSubDirectory(ISimpleFileInfo dirSfi)
        {
            return dirSfi.Children.Any(fi => fi.Type == SfiType.AudioFile);
        }

        #endregion [  private methods  ]
    }
}