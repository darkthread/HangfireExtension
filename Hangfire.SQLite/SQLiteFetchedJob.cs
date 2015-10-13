﻿// This file is part of Hangfire.
// Copyright © 2013-2014 Sergey Odinokov.
// 
// Hangfire is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// Hangfire is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with Hangfire. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Data;
using Hangfire.Storage;
using Hangfire.Annotations;

namespace Hangfire.SQLite
{
    internal class SQLiteFetchedJob : IFetchedJob
    {
        private readonly SQLiteStorage _storage;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public SQLiteFetchedJob(
            [NotNull] SQLiteStorage storage,
            [NotNull] IDbConnection connection,
            [NotNull] IDbTransaction transaction,
            string jobId,
            string queue)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            if (connection == null) throw new ArgumentNullException("connection");
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (jobId == null) throw new ArgumentNullException("jobId");
            if (queue == null) throw new ArgumentNullException("queue");

            _storage = storage;
            _connection = connection;
            _transaction = transaction;

            JobId = jobId;
            Queue = queue;
        }

        public string JobId { get; private set; }
        public string Queue { get; private set; }

        public void RemoveFromQueue()
        {
            _transaction.Commit();
        }

        public void Requeue()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _storage.ReleaseConnection(_connection);
        }
    }
}