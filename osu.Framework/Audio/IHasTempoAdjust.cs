// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Audio
{
    public interface IHasTempoAdjust
    {
        /// <summary>
        /// The tempo this track is playing at, relative to original.
        /// Does not affect frequency (pitch) of audio.
        /// </summary>
        double TempoAdjust { get; set; }
    }
}
