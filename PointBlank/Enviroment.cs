﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PointBlank.Framework.Objects;
using PointBlank.Framework.Wrappers;
using PointBlank.API.DataManagment;
using PointBlank.Services.DetourManager;
using UnityEngine;

namespace PointBlank
{
    internal static class Enviroment // Yes I know it is spelt wrong but Environment already exists so we need to use the name
    {
        public static Dictionary<string, ServiceWrapper> services = new Dictionary<string, ServiceWrapper>(); // The list of services and their properties
        public static Dictionary<string, RuntimeObject> runtimeObjects = new Dictionary<string, RuntimeObject>(); // The list of runtime objects

        public static List<SQLData> SQLConnections = new List<SQLData>(); // The list of all the sql connections

        public static WeebClient WebsiteClient = new WeebClient(); // Website client for pointblank
    }
}
