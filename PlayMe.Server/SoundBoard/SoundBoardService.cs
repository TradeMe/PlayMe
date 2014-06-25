using System;
using System.Collections.Generic;
using System.Linq;
using PlayMe.Data;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Interfaces;
using PlayMe.Server.Player;

namespace PlayMe.Server.SoundBoard
{
   public class SoundBoardService : ISoundBoardService
   {
       private readonly IRepository<SoundBoardInfo> _soundBoardRepository;
       private readonly ISoundBoardSettings soundBoardSettings;
       private readonly IStreamedPlayer player;
       private readonly IPathBuilder pathBuilder;
       private const int minimumVetoes = 2; 
       private readonly IList<string> files = new List<string>()
                                         {
                                             "doublekill.wav",
                                             "multikill.wav",
                                             "megakill.wav",
                                             "ultrakill.wav",
                                             "monsterkill.wav",
                                             "killingspree.wav",
                                             "rampage.wav",
                                             "dominating.wav",
                                             "unstoppable.wav",
                                             "godlike.wav"
                                         };

       private readonly INowHelper nowHelper;
       private bool hasPlayedFinishHimForThisTrack = false;

       public SoundBoardService(IRepository<SoundBoardInfo> _soundBoardRepository, ISoundBoardSettings soundBoardSettings, IStreamedPlayer player, IPathBuilder pathBuilder, INowHelper nowHelper)
       {
           this.nowHelper = nowHelper;
           this.pathBuilder = pathBuilder;
           this.player = player;
           this.soundBoardSettings = soundBoardSettings;
           this._soundBoardRepository = _soundBoardRepository;
       }

       public void PlayFinishHim()
       {
           if (soundBoardSettings.IsEnabled && !hasPlayedFinishHimForThisTrack)
           {
               player.PlayFromFile(pathBuilder.BuildFilePath("finishhim.wav"));
               hasPlayedFinishHimForThisTrack = true;
           }
       }

       public void PlayVetoSound()
       {
           hasPlayedFinishHimForThisTrack = false;
            if (soundBoardSettings.IsEnabled)
            {
                bool firstblood = false;
                var lastVetoDateTime = new DateTime();
                int skipCount = 0;

                var latestVetoInfo = _soundBoardRepository.GetAll().FirstOrDefault();
                if (latestVetoInfo != null)
                {
                    lastVetoDateTime = latestVetoInfo.lastSkippedSongTime;
                    skipCount = latestVetoInfo.skippedSongsCount;
                }
                else
                {
                    latestVetoInfo = new SoundBoardInfo();
                }

                //first veto of the day!
                if (!lastVetoDateTime.ToLocalTime().Date.Equals(nowHelper.Now.Date))
                {
                    firstblood = true;                        
                }

                var n = nowHelper.Now.ToUniversalTime();
                var latestTime = lastVetoDateTime.AddSeconds(soundBoardSettings.SecondsBetweenSkipThreshold);
                if (DateTime.Compare(latestTime, n) > 0)
                {
                    skipCount++;

                    string fileName = GetVetoSound(skipCount);
                    if (!string.IsNullOrEmpty(fileName)) player.PlayFromFile(fileName);                    

                }
                else
                {
                    skipCount = 1;
                }

                latestVetoInfo.lastSkippedSongTime = nowHelper.Now;
                latestVetoInfo.skippedSongsCount = skipCount;

                _soundBoardRepository.InsertOrUpdate(latestVetoInfo);

                //play the first blood sound if its the first veto of the day
                if(firstblood)
                {
                    player.PlayFromFile(pathBuilder.BuildFilePath("firstblood.wav"));
                }
            }
        }
       
       private string GetVetoSound(int vetoCount)
       {
           int index = vetoCount - minimumVetoes;
           if (index >= 0 && index < files.Count)
           {
                return pathBuilder.BuildFilePath(files[index]);
           }
           return string.Empty;
       }

       public void PlayFinishHim(int requiredVetos, QueuedTrack foundTrack)
       {
           if (soundBoardSettings.IsEnabled)
           {
               var howManyVetosToGo = (requiredVetos - foundTrack.VetoCount);

               if (foundTrack.LikeCount > 5 && howManyVetosToGo == 1)
               {
                   player.PlayFromFile(pathBuilder.BuildFilePath("finishhim.wav"));
               }
           }
       }
    }

    
}
