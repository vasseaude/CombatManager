﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CombatManager.Maps
{
    public class GameMapList
    {
        ObservableCollection<MapStub> maps = new ObservableCollection<MapStub>();


        public delegate void MapChangedDelegate(GameMapList.MapStub map);

        public event MapChangedDelegate MapChanged;

        int id;

        public GameMapList()
        {
            maps.CollectionChanged += Maps_CollectionChanged;
        }

        private void Maps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GameMapList.MapStub map in e.NewItems)
                {
                    map.PropertyChanged += Map_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (GameMapList.MapStub map in e.OldItems)
                {
                    map.PropertyChanged -= Map_PropertyChanged;
                }
            }
        }

        private void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (MapChanged != null)
            {
                MapChanged((GameMapList.MapStub)sender);
            }
        }

        public GameMap CreateMap(String name)
        {
            GameMap map = new GameMap(Id++, name);

            MapStub stub = new MapStub(map);

            Maps.Add(stub);
            map.CanSave = true;
            map.SaveMap(true);

            return map;
        }

        public GameMap LoadStub(MapStub stub)
        {
            GameMap map = GameMap.LoadMap(stub.Id);
            stub.Map = map;
            if (stub.SourceFile == null)
            {
                stub.SourceFile = map.SourceFile;
            }

            map.CanSave = true;

            return map;
        }

        public void RemoveMap(MapStub stub)
        {
            Maps.Remove(stub);
            GameMap.Delete(stub.Id);
        }


        public class MapStub : SimpleNotifyClass
        {
            String name;
            String sourceFile;
            int id;
            GameMap map;

            public MapStub() { }
            public MapStub(GameMap map)
            {
                name = map.Name;
                id = map.Id;
                this.map = map;
                this.sourceFile = map.SourceFile;

            }
            public string Name
            {
                get
                {
                    return name;
                }

                set
                {
                    name = value;
                    Notify("Name");
                }
            }

            public int Id
            {
                get
                {
                    return id;
                }

                set
                {
                    id = value;
                    Notify("Id");
                }
            }

            [XmlIgnore]
            public GameMap Map
            {
                get
                {
                    return map;
                }

                set
                {
                    if (map != value)
                    {
                        map = value;
                        map.PropertyChanged += Map_PropertyChanged;
                        Notify("Map");
                    }
                }
            }

            public string SourceFile
            {
                get
                {
                    return sourceFile;
                }

                set
                {
                    sourceFile = value;
                }
            }

            private void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "Name")
                {
                    name = map.Name;
                    Notify("Name");
                }
                else if (e.PropertyName == "SourceFile")
                {
                    sourceFile = map.SourceFile;
                    Notify("SourceFile");
                }
            }
        }

        public ObservableCollection<MapStub> Maps
        {
            get
            {
                return maps;
            }
            set
            {
                maps = value;
            }
        }
             


        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }
    }
}
