﻿using PicView.ImageHandling;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static PicView.ChangeImage.Navigation;
using static PicView.Library.Fields;

namespace PicView.ChangeImage
{
    /// <summary>
    /// Used for containing a list of BitmapSources
    /// </summary>
    internal static class Preloader
    {

        /// <summary>
        /// Preloader list of BitmapSources
        /// </summary>
        private static readonly ConcurrentDictionary<
            string,
            BitmapSource> Sources = new ConcurrentDictionary<string, BitmapSource>();

        /// <summary>
        /// Add file to prelader
        /// </summary>
        /// <param name="file">file path</param>
        internal static Task Add(string file)
        {
            return Task.Run(() =>
            {
                var pic = ImageDecoder.RenderToBitmapSource(file);               

#if DEBUG
                Trace.WriteLine($"Added {file} to Preloader, index {Pics.IndexOf(file)}");
#endif

                Sources.TryAdd(file, pic);
            });
        }

        /// <summary>
        /// Add file to preloader from index
        /// </summary>
        /// <param name="i">Index of Pics</param>
        internal static void Add(int i)
        {
            if (i >= Pics.Count || i < 0)
            {
                return;
            }

            if (File.Exists(Pics[i]))
            {
                if (!Contains(Pics[i]))
                {
                    Add(Pics[i]);
                }
                else
                {
#if DEBUG
                    Trace.WriteLine("Skipped at index " + i);
#endif
                }
            }
            else
            {
                Pics.Remove(Pics[i]);

#if DEBUG
                Trace.WriteLine($"Preloader removed: {Pics[i]} from Pics, index {i}");
#endif
            }
        }

        internal static void Add(BitmapSource bmp, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
#if DEBUG
                Trace.WriteLine("Preloader.Add key is null");
#endif
                return;
            }

            if (Contains(key))
            {
#if DEBUG
                Trace.WriteLine("Preloader.Add already contains " + key);
#endif
                return;
            }
#if DEBUG
            if (Sources.TryAdd(key, bmp))
            {
                Trace.WriteLine("Manually added = " + key + " to Preloader, index " + Pics.IndexOf(key));
            }
            else
            {
                Trace.WriteLine("Preloader failed to add = " + key + " , index " + Pics.IndexOf(key));
            }
#else
            Sources.TryAdd(key, bmp);
#endif
        }

        /// <summary>
        /// Removes the key, after checking if it exists
        /// </summary>
        /// <param name="key"></param>
        internal static void Remove(string key)
        {
            if (key == null)
            {
#if DEBUG
                Trace.WriteLine("Preloader.Remove key null, " + key);
#endif
                return;
            }

            if (!Contains(key))
            {
#if DEBUG
                Trace.WriteLine("Preloader.Remove does not contain " + key);
#endif
                return;
            }

            _ = Sources[key];
#if DEBUG
            if (Sources.TryRemove(key, out _))
            {
                Trace.WriteLine($"Removed {key} from Preloader, index {Pics.IndexOf(key)}");
            }
            else
            {
                Trace.WriteLine($"Failed to Remove {key} from Preloader, index {Pics.IndexOf(key)}");
            }
#else
            Sources.TryRemove(key, out _);
#endif
        }

        /// <summary>
        /// Removes the key, after checking if it exists
        /// </summary>
        /// <param name="key"></param>
        internal static void Remove(int i)
        {
            if (i >= Pics.Count || i < 0)
            {
                return;
            }

            if (File.Exists(Pics[i]))
            {
                if (Contains(Pics[i]))
                {
                    Remove(Pics[i]);
                }
            }
        }

        /// <summary>
        /// Removes all keys
        /// </summary>
        internal static void Clear()
        {
            if (Sources.Count <= 0)
            {
                return;
            }

            Sources.Clear();
#if DEBUG
            Trace.WriteLine("Cleared Preloader");
#endif
        }

        /// <summary>
        /// Removes specific keys
        /// </summary>
        /// <param name="array"></param>
        internal static void Clear(string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Remove(array[i]);
#if DEBUG
                Trace.WriteLine($"Removed = {array[i]} from Preloader");
#endif
            }
        }

        /// <summary>
        /// Returns the specified BitmapSource.
        /// Returns null if key not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static BitmapSource Load(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || !Contains(key))
            {
                return null;
            }

            return Sources[key];
        }

        /// <summary>
        /// Checks if the specified key exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static bool Contains(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || Sources.Count <= 0)
            {
                return false;
            }

            return Sources.ContainsKey(key);
        }

        internal static void PreloaderFix(string file)
        {
            if (!Contains(file))
            {
                Add(file);
            }
        }

        /// <summary>
        /// Starts decoding images into memory,
        /// based on current index and if reverse or not
        /// </summary>
        /// <param name="index"></param>
        /// <param name="reverse"></param>
        internal static Task PreLoad(int index)
        {
#if DEBUG
            Trace.WriteLine("Preolader started, "
                + string.Concat(Properties.Settings.Default.Looping ? "looping " : string.Empty)
                + string.Concat(Reverse ? "backwards" : "forwards"));
#endif

            return Task.Run(() =>
            {
                var toLoad = 3;
                var extraToLoad = 2;
                var cleanUp = toLoad + extraToLoad;

                // Not looping
                if (!Properties.Settings.Default.Looping)
                {
                    // Forwards
                    if (!Reverse)
                    {
                        // Add first elements
                        for (int i = index + 1; i < index + 1 + toLoad; i++)
                        {
                            if (i > Pics.Count)
                            {
                                break;
                            }

                            Add(i);
                        }
                        // Add second elements behind
                        for (int i = index - 1; i > index - 1 - extraToLoad; i--)
                        {
                            if (i < 0)
                            {
                                break;
                            }

                            Add(i);
                        }

                        //Clean up behind
                        if (Pics.Count > cleanUp * 2 && !FreshStartup)
                        {
                            for (int i = index - 1 - cleanUp; i < (index - 1 - extraToLoad); i++)
                            {
                                if (i < 0)
                                {
                                    continue;
                                }

                                if (i > Pics.Count)
                                {
                                    break;
                                }

                                Remove(i);
                            }
                        }
                    }
                    // Backwards
                    else
                    {
                        // Add first elements behind
                        for (int i = index - 1; i > index - 1 - toLoad; i--)
                        {
                            if (i < 0)
                            {
                                break;
                            }

                            Add(i);
                        }
                        // Add second elements
                        for (int i = index + 1; i <= index + 1 + toLoad; i++)
                        {
                            if (i > Pics.Count)
                            {
                                break;
                            }

                            Add(i);
                        }

                        //Clean up infront
                        if (Pics.Count > cleanUp * 2 && !FreshStartup)
                        {
                            for (int i = index + 1 + cleanUp; i > index + 1 + cleanUp - extraToLoad; i--)
                            {
                                if (i < 0)
                                {
                                    continue;
                                }

                                if (i > Pics.Count)
                                {
                                    break;
                                }

                                Remove(i);
                            }
                        }
                    }
                }

                // Looping!
                else
                {
                    // Forwards
                    if (!Reverse)
                    {
                        // Add first elements
                        for (int i = index + 1; i < index + 1 + toLoad; i++)
                        {
                            if (i != 0 || Pics.Count != 0)
                            {
                                Add(i % Pics.Count);
                            }
                        }
                        // Add second elements behind
                        for (int i = index - 1; i > index - 1 - extraToLoad; i--)
                        {
                            if (i != 0 || Pics.Count != 0)
                            {
                                Add(i % Pics.Count);
                            }
                        }

                        //Clean up behind
                        if (Pics.Count > cleanUp * 2 && !FreshStartup)
                        {
                            for (int i = index - 1 - cleanUp; i < (index - 1 - extraToLoad); i++)
                            {
                                Remove(i % Pics.Count);
                            }
                        }
                    }
                    // Backwards
                    else
                    {
                        // Add first elements behind
                        int y = 0;
                        for (int i = index - 1; i > index - 1 - toLoad; i--)
                        {
                            y++;
                            if (i < 0)
                            {
                                for (int x = Pics.Count - 1; x >= Pics.Count - y; x--)
                                {
                                    Add(x);
                                }
                                break;
                            }
                            Add(i);
                        }

                        // Add second elements
                        for (int i = index + 1; i <= index + 1 + toLoad; i++)
                        {
                            if (i != 0 || Pics.Count != 0)
                            {
                                Add(i % Pics.Count);
                            }
                        }

                        //Clean up infront
                        if (Pics.Count > cleanUp + toLoad && !FreshStartup)
                        {
                            for (int i = index + 1 + cleanUp; i > index + 1 + cleanUp - extraToLoad; i--)
                            {
                                if (i != 0 || Pics.Count != 0)
                                {
                                    Remove(i % Pics.Count);
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}