# Vistian.Reactive.Paging

Data paging for ReactiveUI based solutions, functioning in a reactive way.

## Capabilities

1. Read only collection with data loaded through an application supplied  observable.
2. Customizable mechanisms for not only deciding when to page data but also around retention of data.
3. Out of the box ReactiveList and SourceList providers (caveat around SourceList though, see later).
4. Data changes can be observed and correctly marshalled to the UI automatically.
5. Support for contiguous reads of paged data.
6. Busy observable for when paged reads are taking place.
7. Custom collection item instances pending read of paged data.

## Quick Start

1. Add the collection for which you want the data to be paged to your view model

`[Reactive]
public ReadOnlyBackingStoreCollection<SimpleModel> List {get; set; }`

2. In the constructor of the view model set the relationship between the above collection and your provider of data, either using one of the provided templated solutions, or more explicitly.

```
    // MyPageReadingObservable loads the data in pages, 10 is the max size of the page of data to read, HandleException is called when an exception occurs. Handle Exception is an Observable that if it returns true forces the read operation to occur again

    List = ReadOnlyBackingStoreCollection.FromKeyedReactiveListPager( MyPageReadingObservable, (s) => s.Key, 10, HandleException); 
```

    3. Add your paged reading observable code and the optional exception handler.
```
    private IObservable<bool> HandleException(PageReadRequest readRequest, Exception exception)
    {
        ...
        // return true to retry the read again, otherwise false.
    }

    private IObservable<PageReadResult<SimpleModel>> MyPageReadingObservable(PageReadRequest request)
    {
        return Observable.Create<PageReadResult<SimpleModel>>((s) =>
        {
        ... actual read code here
        });
    }
```

4. In the UI bind the collection in exactly the way you normally would.


## How it works

The lowest level assumes that there is a provider of paged data through a standard **IPagingController** implementation. The library provides a few standard implementations of this controller which take an Observable to handle the reading of paged data.

The View Model would contain a fascade of an observable collection, **ReadOnlyBackingStoreCollection**. This collection in fact doesn't hold any data at all, but instead does a few things:
1. Uses an implementation of a **ICollectionItemTouchedStrategy** to indicate  when a specific index in the collection has been referenced.
2. Uses a **IPagedBackingStoreCollection** implementation to actually provide content.
3. Observes changes in the IPagedBackingStoreCollection and re-raises these events in the ReadOnlyBackingStoreCollection. These events flag to the UI that the underlying data has changed.


There is a simplistic implementation of ICollectionItemTouchedStrategy which requests the reading of a page when an item is referenced that isn't present in the backing store.

Similarly, there is an implementation of IPagedBackingStoreCollection, **SimpleBackingStoreCollection** which maintains a collection of all of the data read so far.

It should be noted that whilst the current implementation is focussed around a fascade collection which is bound to, the library is a 'bag of parts' which should be easily composible to differing solutions should that be required.

Similarly it should also be noted that given that much of the data changes are provided through observable sequences of change sets the capability for refreshed data to make its way automatically through to the UI should be possible.
To implement a virtualization solution, an alternative implementation of IPagedBackingStoreCollection would need to be implemented, everything else would remain the same.

## To be done

1. Not a lot of testing has been done with the library. It is currently being used in an Android solution (RecyclerView based) but the same views have not yet been implemented on iOS.
2. As mentioned about there is a slight caveat around SourceList which may not affect many, but it is something to be aware of. For us we needed that one of the low level ChangeSetProviders needed a 'reload' capability. Whilst SourceList does allow for the application of replacement data, it doesn't guarantee that the order will be correct. There would appear to be an implicit assumption in Dynamic Data that ordering is done post 'Source'. I believe there may be a work around by introducing an 'inbetween' class with position contained within it.  For now we just run with ReactiveList instead.
3. Unit tests need better coverage.
4. Provide virtualized backing store with potentially customizable 'page out'/dispose strategy.