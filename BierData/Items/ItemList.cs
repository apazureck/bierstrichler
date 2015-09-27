using Bierstrichler.Data.Events;
using Bierstrichler.Data.Global;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Bierstrichler.Data.Items
{
    public class ItemList : IList<Item>
    {
        List<Item> list = new List<Item>();
        public event ItemChangedEventHandler ItemAdded;
        public event ItemChangedEventHandler ItemRemoved;
        public event EventHandler HistoryCleared;
        public event EventHandler FileSaved;
        private System.Timers.Timer saveDelayTimer;

        string ItemsFile { get; set; }
        string BackupFile { get; set; }

        public ItemList()
        {
            saveDelayTimer = new System.Timers.Timer(50);
            saveDelayTimer.Elapsed += saveDelayTimer_Elapsed;
        }

        void saveDelayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            saveDelayTimer.Stop();
            SaveItemList();
        }

        public ItemList(List<Item> origin) : this()
        {
            list = origin;
        }

        public void Add(Item item)
        {
            list.Add(item);
            if (ItemAdded != null)
                ItemAdded(this, new ItemEventArgs(item));
            //SaveItemsListAsync();
        }

        public bool Remove(Item item)
        {
            bool removed = list.Remove(item);
            if (ItemRemoved != null)
                ItemRemoved(this, new ItemEventArgs(item));
            //SaveItemsListAsync();
            return removed;
        }

        public void Insert(int index, Item item)
        {
            list.Insert(index, item);
            if (ItemAdded != null)
                ItemAdded(this, new ItemEventArgs(item, index));
            //SaveItemsListAsync();
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
            if (ItemRemoved != null)
                ItemRemoved(this, new ItemEventArgs(null, index));
            //SaveItemsListAsync();
        }

        public int IndexOf(Item item)
        {
            return list.IndexOf(item);
        }

        void IList<Item>.Insert(int index, Item item)
        {
            list.Insert(index, item);
            if (ItemAdded != null)
                ItemAdded(this, new ItemEventArgs(item, index));
            //SaveItemsListAsync();
        }

        void IList<Item>.RemoveAt(int index)
        {
            list.RemoveAt(index);
            if (ItemRemoved != null)
                ItemRemoved(this, new ItemEventArgs(null, index));
            //SaveItemsListAsync();
        }

        public Item this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
                if (ItemAdded != null)
                    ItemAdded(this, new ItemEventArgs(list[index], index));
                //SaveItemsListAsync();
            }
        }

        void ICollection<Item>.Add(Item item)
        {
            this.Add(item);
        }

        public void Clear()
        {
            List<Item> thislist = new List<Item>(list);
            list.Clear();
            int i = 0;
            foreach(Item p in thislist)
                if (ItemRemoved != null)
                    ItemRemoved(this, new ItemEventArgs(p, i++));
            //SaveItemsListAsync();
        }

        public bool Contains(Item item)
        {
            return list.Contains(item);
        }

        public void CopyTo(Item[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<Item>.Remove(Item item)
        {
            return this.Remove(item);
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public Item Find(Predicate<Item> match)
        {
            return list.Find(match);
        }

        public IList<Item> FindAll(Predicate<Item> match)
        {
            return list.FindAll(match);
        }

        internal void SaveToFile(string itemsFile)
        {
            //rwLock.AcquireWriterLock(-1);
            //using (FileStream fs = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), itemsFile), FileMode.OpenOrCreate))
            //{
            //    BinaryFormatter bf = new BinaryFormatter();
            //    bf.Serialize(fs, list);
            //}
            //rwLock.ReleaseLock();

            try
            {
                rwLock.AcquireWriterLock(-1);
                for (int i = 0; i < 3; i++)
                    try
                    {
                        using (FileStream fs = new FileStream(itemsFile, FileMode.OpenOrCreate))
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            if (this.Count > 0)
                                bf.Serialize(fs, list);
                        }
                    }
                    catch (Exception e)
                    {
                        if (i == 2)
                        {
                            MessageBox.Show("Stellen Sie sicher, dass der Dateipfad vorhanden ist.", "items.dat konnte nicht gespeichert werden. Ursache:" + e.Message);
                            Log.WriteErrorFromException("Items konnten nicht gespeichert werden.", e);
                            throw;
                        }
                        Thread.Sleep(1000);
                    }
                if (FileSaved != null)
                    FileSaved(this, null);
            }
            finally
            {
                rwLock.ReleaseLock();
            }
        }

        ReaderWriterLock rwLock = new ReaderWriterLock();

        public void ClearHistory()
        {
            
            foreach (Item i in list)
                i.Changes.Clear();
            if (HistoryCleared != null)
                HistoryCleared(this, null);
            
        }

        public void SaveItemListAsync()
        {
            if (saveDelayTimer.Enabled)
            {
                saveDelayTimer.Stop();
                saveDelayTimer.Start();
            }
            else
                saveDelayTimer.Start();
        }

        public void SaveItemList()
        {
            SaveToFile(ItemsFile);
            if (!string.IsNullOrEmpty(BackupFile))
                SaveToFile(BackupFile);
        }

        public static ItemList LoadFromFile(string itemsFile, string backupFile = null)
        {
            try
                {
                    Log.WriteInformation("Opening Item List.");
                    using (FileStream fs = new FileStream(itemsFile, FileMode.Open))
                    {
                        BinaryFormatter bf = new BinaryFormatter();

                        return new ItemList((List<Item>)bf.Deserialize(fs))
                        {
                            ItemsFile = itemsFile,
                            BackupFile = backupFile
                        };

                    }
                }
                catch (Exception ex)
                {
                    Log.WriteWarningFromException("Could not load Item List.", ex);
                    return new ItemList()
                    {
                        ItemsFile = itemsFile,
                        BackupFile = backupFile
                    };
                }
        }
    }
}
