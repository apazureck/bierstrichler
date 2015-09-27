using Bierstrichler.Data.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Windows;
using Bierstrichler.Data.Events;

namespace Bierstrichler.Data.Persons
{
    [Serializable]
    public class PersonList : IList<Person>
    {
        List<Person> list = new List<Person>();
        public event PersonChangedEventHandler PersonAdded;
        public event PersonChangedEventHandler PersonRemoved;
        public event EventHandler HistoryCleared;
        public event EventHandler FileSaved;
        ReaderWriterLock rwLock = new ReaderWriterLock();
        System.Timers.Timer saveDelayTimer;

        public string BackupFile { get; set; }
        private string PersonsFile { get; set; }

        public PersonList()
        {
            saveDelayTimer = new System.Timers.Timer(50);
            saveDelayTimer.Elapsed += saveDelayTimer_Elapsed;
        }

        void saveDelayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            saveDelayTimer.Stop();
            SavePersonsList();
        }

        public PersonList(List<Person> origin) : this()
        {
            list = origin;
        }

        public void Add(Person item)
        {
            list.Add(item);
            if (PersonAdded != null)
                PersonAdded(this, new PersonEventArgs(item));
            SavePersonsListAsync();
        }

        public bool Remove(Person item)
        {
            bool removed = list.Remove(item);
            if (PersonRemoved != null)
                PersonRemoved(this, new PersonEventArgs(item));
            SavePersonsListAsync();
            return removed;
        }

        public void Insert(int index, Person item)
        {
            list.Insert(index, item);
            if (PersonAdded != null)
                PersonAdded(this, new PersonEventArgs(item, index));
            SavePersonsListAsync();
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
            if (PersonRemoved != null)
                PersonRemoved(this, new PersonEventArgs(null, index));
            SavePersonsListAsync();
        }

        public void SavePersonsListAsync()
        {
            if (saveDelayTimer.Enabled)
            {
                saveDelayTimer.Stop();
                saveDelayTimer.Start();
            }
            else
                saveDelayTimer.Start();
        }

        public void SavePersonsList()
        {
            SavePersonsList(PersonsFile);
            if (!string.IsNullOrEmpty(BackupFile))
                SavePersonsList(BackupFile);
        }

        private void SavePersonsList(string path)
        {
            try
            {
                rwLock.AcquireWriterLock(-1);
                for (int i = 0; i < 3; i++)
                    try
                    {
                        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            if (this.Count > 0)
                                bf.Serialize(fs, list);
                        }
                        break;
                    }
                    catch (Exception e)
                    {
                        if (i == 2)
                        {
                            MessageBox.Show("Stellen Sie sicher, dass der Dateipfad vorhanden ist.", "persons.dat konnte nicht gespeichert werden. Ursache:" + e.Message);
                            Log.WriteErrorFromException("Personen konnten nicht gespeichert werden.", e);
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

        public static PersonList LoadFromFile(string personsFile, string backupFile = null)
        {
            List<Person> list;
            try
            {
                Log.WriteInformation("Opening Person List.");
                using (FileStream fs = new FileStream(personsFile, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    list = (List<Person>)bf.Deserialize(fs);
                    return new PersonList(list)
                        {
                            PersonsFile = personsFile,
                            BackupFile = backupFile
                        };
                }
            }
            catch (Exception ex)
            {
                Log.WriteWarningFromException("Could not load Person List.", ex);
                return new PersonList()
                {
                    PersonsFile = personsFile,
                    BackupFile = backupFile
                };
            }
            
        }

        public int IndexOf(Person item)
        {
            return list.IndexOf(item);
        }

        void IList<Person>.Insert(int index, Person item)
        {
            list.Insert(index, item);
            if (PersonAdded != null)
                PersonAdded(this, new PersonEventArgs(item, index));
            SavePersonsListAsync();
        }

        void IList<Person>.RemoveAt(int index)
        {
            list.RemoveAt(index);
            if (PersonRemoved != null)
                PersonRemoved(this, new PersonEventArgs(null, index));
            SavePersonsListAsync();
        }

        public Person this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
                if (PersonAdded != null)
                    PersonAdded(this, new PersonEventArgs(list[index], index));
                SavePersonsListAsync();
            }
        }

        void ICollection<Person>.Add(Person item)
        {
            this.Add(item);
        }

        public void Clear()
        {
            List<Person> thislist = new List<Person>(list);
            list.Clear();
            int i = 0;
            foreach(Person p in thislist)
                if (PersonRemoved != null)
                    PersonRemoved(this, new PersonEventArgs(p, i++));
            SavePersonsListAsync();
        }

        public bool Contains(Person item)
        {
            return list.Contains(item);
        }

        public void CopyTo(Person[] array, int arrayIndex)
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

        bool ICollection<Person>.Remove(Person item)
        {
            return this.Remove(item);
        }

        public IEnumerator<Person> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public Person Find(Predicate<Person> match)
        {
            return list.Find(match);
        }

        public IList<Person> FindAll(Predicate<Person> match)
        {
            return list.FindAll(match);
        }

        public void ClearHistory()
        {
            foreach (Consumer c in list)
                c.History.Clear();
            if (HistoryCleared != null)
                HistoryCleared(this, null);
        }
    }
}
